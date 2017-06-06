using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TimeMiner.Core;
using TimeMiner.Core.Plugging;

namespace TimeMiner.Slave
{
    /// <summary>
    /// Class for communication with master
    /// </summary>
    class MasterBoundary
    {
        #region singletone
        private static MasterBoundary self;

        public static MasterBoundary Self
        {
            get
            {
                if (self == null)
                {
                    self = new MasterBoundary();
                }
                return self;
            }
        }
        #endregion
        
        /// <summary>
        /// Url to master
        /// </summary>
        private readonly string LOG_SEND_URL = "http://" + ConfigManager.Self.Server + ":" + ConfigManager.Self.SendPort;

        private MasterBoundary()
        {

        }
        
        private async Task<byte[]> SendBytesToServer(byte[] data)
        {
            try
            {
                HttpWebRequest req = HttpWebRequest.CreateHttp(LOG_SEND_URL);
                req.Method = "POST";
                var stream = await req.GetRequestStreamAsync();
                await stream.WriteAsync(data, 0, data.Length);
                stream.Close();
                var resp = (HttpWebResponse) await req.GetResponseAsync();
                if (resp.StatusCode != HttpStatusCode.OK)
                    return null;
                var respStream = resp.GetResponseStream();
                using (var ms = new MemoryStream())
                {
                    await respStream.CopyToAsync(ms);
                    respStream.Close();
                    return ms.ToArray();
                }
            }
            catch (Exception e)
            {
                //failed to send data
                return null;
            }

        }
        /// <summary>
        /// Send one record immediately
        /// </summary>
        /// <param name="record">Record to send</param>
        /// <returns></returns>
        public async Task<bool> SendOne(LogRecord record)
        {
            string json = JsonConvert.SerializeObject(record, Formatting.Indented);
            //File.AppendAllText("log.txt",data + "\r\n");
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] resp = await SendBytesToServer(data);
            if (resp == null)
            {
                Console.WriteLine("Not sent");
                Console.WriteLine("-");
            }
            else
            {
                Console.WriteLine("Sent");
                Console.WriteLine(resp.Length);
            }
            return resp != null;
        }
        /// <summary>
        /// Sends many records in one transaction
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public async Task<bool> SendMany(LogRecord[] records)
        {
            string json = JsonConvert.SerializeObject(records);
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] resp = await SendBytesToServer(data);
            if (resp == null)
            {
                Console.WriteLine("Not sent");
                Console.WriteLine("-");

            }
            else
            {
                Console.WriteLine("Sent many");
                Console.WriteLine(resp.Length);
            }
            return resp != null;
        }
        /// <summary>
        /// Get plugin descriptors from master server
        /// </summary>
        /// <returns></returns>
        public async Task<PluginDescriptor[]> GetPluginDescriptors()
        {
            try
            {
                string URL = "http://" + ConfigManager.Self.Server + ":" + ConfigManager.Self.ApiPort + "/api/slave/plugins/slave_getlist";
                HttpWebRequest req = WebRequest.CreateHttp(URL);
                HttpWebResponse resp = (HttpWebResponse)await req.GetResponseAsync();
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    string json = await sr.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<PluginDescriptor[]>(json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        /// <summary>
        /// Download assembly from master server
        /// </summary>
        /// <param name="guid">Guid of assembly</param>
        /// <returns></returns>
        public async Task<byte[]> GetAssembly(Guid guid)
        {
            try
            {
                string URL = "http://" + ConfigManager.Self.Server + ":" + ConfigManager.Self.ApiPort + "/api/slave/plugins/slave_getassembly";
                HttpWebRequest req = WebRequest.CreateHttp(URL);
                req.Method = "POST";
                using (StreamWriter sw = new StreamWriter(await req.GetRequestStreamAsync()))
                {
                    var reqObj = new
                    {
                        Guid = guid
                    };
                    var json = JsonConvert.SerializeObject(reqObj);
                    await sw.WriteLineAsync(json);
                    sw.Close();
                }
                HttpWebResponse resp = (HttpWebResponse)await req.GetResponseAsync();
                MemoryStream ms = new MemoryStream();
                var respStream = resp.GetResponseStream();
                if (respStream == null)
                    return null;
                await respStream.CopyToAsync(ms);
                return ms.ToArray();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public enum Relevance
        {
            good,
            neutral,
            bad,
            unknown
        }

        public async Task<Relevance> GetLastStatus()
        {
            try
            {
                string URL = "http://" + ConfigManager.Self.Server + ":" + ConfigManager.Self.ApiPort + "/api/slave/get_now_status";
                HttpWebRequest req = WebRequest.CreateHttp(URL);
                req.Method = "POST";
                using (StreamWriter sw = new StreamWriter(await req.GetRequestStreamAsync()))
                {
                    var reqObj = new
                    {
                        Guid = ConfigManager.Self.UserId
                    };
                    var json = JsonConvert.SerializeObject(reqObj);
                    await sw.WriteLineAsync(json);
                    sw.Close();
                }
                HttpWebResponse resp = (HttpWebResponse)await req.GetResponseAsync();
                var respStream = resp.GetResponseStream();
                if (respStream == null)
                    return Relevance.unknown;
                string respJson = "";
                using (var sr = new StreamReader(respStream))
                {
                    respJson = await sr.ReadToEndAsync();
                    sr.Close();
                }
                var jobj = JObject.Parse(respJson);
                var rel = (Relevance)jobj["Rel"].Value<int>();
                return rel;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Relevance.unknown;
            }
        }
    }
}
