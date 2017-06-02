using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        /// Delegate for handling RecordSent event
        /// </summary>
        /// <param name="record">Record, that was sent</param>
        public delegate void OnRecordSentHandler(LogRecord record);
        /// <summary>
        /// Event, that happens when record was sent to master
        /// </summary>
        public event OnRecordSentHandler onRecordSent;

        /// <summary>
        /// Url to master
        /// </summary>
        private readonly string LOG_SEND_URL = "http://" + ConfigManager.Self.Server;//"http://localhost:13000";

        private MasterBoundary()
        {
            
        }
        /// <summary>
        /// Raise on record sent event
        /// </summary>
        /// <param name="recrd"></param>
        private void RaiseOnRecordSent(LogRecord recrd)
        {
            if (onRecordSent != null)
            {
                onRecordSent(recrd);
            }
        }
        /// <summary>
        /// Send one record immediately
        /// </summary>
        /// <param name="record">Record to send</param>
        /// <returns></returns>
        public async Task SendOne(LogRecord record)
        {
            try
            {
                string data = JsonConvert.SerializeObject(record, Formatting.Indented);
                File.AppendAllText("log.txt",data + "\r\n");
                HttpWebRequest req = HttpWebRequest.CreateHttp(LOG_SEND_URL);
                req.Method = "POST";
                using (StreamWriter sw = new StreamWriter(await req.GetRequestStreamAsync()))
                {
                    await sw.WriteLineAsync(data);
                    sw.Close();
                    HttpWebResponse resp = (HttpWebResponse)(await req.GetResponseAsync());
                    if (resp.StatusCode == HttpStatusCode.OK)
                    {
                        //successfully sent
                        Console.WriteLine("Sent!");
                        Console.WriteLine("-");
                        RaiseOnRecordSent(record);
                    }
                    else
                    {
                        Console.WriteLine("Not sent!");
                        Console.WriteLine("-");
                    }
                    //log was not successfully sent
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception!");
            }
        }
        /// <summary>
        /// Get plugin descriptors from master server
        /// </summary>
        /// <returns></returns>
        public async Task<PluginDescriptor[]> GetPluginDescriptors()
        {
            try
            {
                string URL = "http://" + "localhost" + ":" + "8080" + "/api/slave/plugins/slave_getlist";
                HttpWebRequest req = WebRequest.CreateHttp(URL);
                HttpWebResponse resp = (HttpWebResponse) await req.GetResponseAsync();
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    string json = sr.ReadToEnd();
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
                string URL = "http://" + "localhost" + ":" + "8080" + "/api/slave/plugins/slave_getassembly";
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
    }
}
