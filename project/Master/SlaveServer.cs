using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TimeMiner.Core;

namespace TimeMiner.Master
{
    /// <summary>
    /// This is the server for communication with slaves
    /// </summary>
    class SlaveServer
    {
        private static SlaveServer self;

        public static SlaveServer Self
        {
            get
            {
                if (self == null)
                {
                    self = new SlaveServer();
                }
                return self;
            }
        }

        public const string LISTENER_PORT = "13000";
        public const string LISTENER_PREFIX = "http://+:" + LISTENER_PORT + "/";

        public event Action<LogRecord> onLogRecordCame;
        public event Action<LogRecord[]> onManyLogRecordsCame;
        private HttpListener listener;
        //const string SERVER
        private SlaveServer()
        {
            //setup listener
            listener = new HttpListener();
            listener.Prefixes.Add(LISTENER_PREFIX);
        }

        /// <summary>
        /// Task, which handles requests from slaves
        /// </summary>
        private Task handlerTask;
        /// <summary>
        /// Start the server
        /// </summary>
        public void Start()
        {
            listener.Start();
            if (handlerTask == null)
            {
                handlerTask = Handler();
            }
        }
        /// <summary>
        /// Handler task that handles incoming requests
        /// </summary>
        /// <returns></returns>
        private async Task Handler()
        {
            while (true)
            {
                try
                {
                    HttpListenerContext context = null;
                    try
                    {
                        context = await listener.GetContextAsync();
                    }
                    catch (InvalidOperationException ee)
                    {
                        Console.WriteLine("stopped listening");
                        break;
                    }
                    var req = context.Request;
                    var resp = context.Response;
                    
                    string str = "";
                    using (StreamReader sr = new StreamReader(req.InputStream, Encoding.UTF8))
                    {
                        str = sr.ReadToEnd();
                    }
                    //File.AppendAllText("log.txt", str + "\r\n"); //FOR DEBUG
                    resp.OutputStream.Close();

                    JToken jtoken = JToken.Parse(str);
                    if (jtoken is JArray)
                    {
                        //many log records came
                        LogRecord[] records = JsonConvert.DeserializeObject<LogRecord[]>(str);
                        onManyLogRecordsCame?.Invoke(records);
                    }
                    if (jtoken is JObject)
                    {
                        LogRecord rec = JsonConvert.DeserializeObject<LogRecord>(str);
                        onLogRecordCame?.Invoke(rec);
                    }
                    /*LogRecord rec = DeserializeLogRecord(str);
                    RaiseOnLogRecordCame(rec);*/
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        /// <summary>
        /// Stop the server
        /// </summary>
        public void Stop()
        {
            listener.Stop();
        }
    }
}
