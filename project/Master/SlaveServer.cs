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

        public delegate void OnLogRecordCameHandler(LogRecord rec);

        const string LISTENER_PREFIX = "http://localhost:13000/";

        public event OnLogRecordCameHandler onLogRecordCame;
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
                HttpListenerContext context = await listener.GetContextAsync();
                var req = context.Request;
                var resp = context.Response;

                string str = "";
                using (StreamReader sr = new StreamReader(req.InputStream,Encoding.UTF8))
                {
                    str = sr.ReadToEnd();
                }
                File.AppendAllText("log.txt",str + "\r\n"); //FOR DEBUG
                resp.OutputStream.Close();
                LogRecord rec = DeserializeLogRecord(str);
                RaiseOnLogRecordCame(rec);
            }
        }
        /// <summary>
        /// Stop the server
        /// </summary>
        public void Stop()
        {
            listener.Stop();
        }
        /// <summary>
        /// Raise onLogRecordCame event if it is not null
        /// </summary>
        /// <param name="rec"></param>
        private void RaiseOnLogRecordCame(LogRecord rec)
        {
            if (onLogRecordCame != null)
            {
                onLogRecordCame(rec);
            }
        }

        /// <summary>
        /// Deserialize log record from string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private LogRecord DeserializeLogRecord(string str)
        {
            return JsonConvert.DeserializeObject<LogRecord>(str);
        }

    }
}
