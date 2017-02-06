using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master
{
    /// <summary>
    /// Server for admin frontend page
    /// </summary>
    class FrontendServer
    {
        #region singletone
        private static FrontendServer self;
        public static FrontendServer Self
        {
            get
            {
                if (self == null)
                {
                    self = new FrontendServer();
                }
                return self;
            }
        }
        #endregion
        /// <summary>
        /// Listener for incoming requests
        /// </summary>
        private HttpListener listener;
        /// <summary>
        /// Task handling incoming requests from listener
        /// </summary>
        private Task handlerTask;

        public const string LISTENER_PORT = "8080";
        /// <summary>
        /// Local server prefix including port
        /// </summary>
        public const string LISTENER_PREFIX = "http://+:" + LISTENER_PORT + "/";//"http://localhost:80/";

        private FrontendServer()
        {
            listener = new HttpListener();
            listener.Prefixes.Add(LISTENER_PREFIX);
        }
        /// <summary>
        /// Start local server
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
        /// Stop local server
        /// </summary>
        public void Stop()
        {
            listener.Stop();
        }
        
        /// <summary>
        /// Handles incoming requests from listener
        /// </summary>
        /// <returns></returns>
        private async Task Handler()
        {
            while (true)
            {
                //if server is not listening anymore - exit
                if (!listener.IsListening)
                {
                    handlerTask = null;
                    break;
                }
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest req = context.Request;
                HttpListenerResponse resp = context.Response;
                HandleRequest(req, resp);
            }
        }
        /// <summary>
        /// Handle request
        /// </summary>
        /// <param name="req">Request</param>
        /// <param name="resp">Response</param>
        private void HandleRequest(HttpListenerRequest req, HttpListenerResponse resp)
        {
            StreamWriter sw = new StreamWriter(resp.OutputStream);
            sw.WriteLine("hello here");
            sw.Close();
        }
    }
}
