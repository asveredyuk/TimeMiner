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
        /// <summary>
        /// Container with files from www folder
        /// </summary>
        private ZipResourceContainer resources;

        public const string LISTENER_PORT = "8080";
        /// <summary>
        /// Local server prefix including port
        /// </summary>
        public const string LISTENER_PREFIX = "http://+:" + LISTENER_PORT + "/";//"http://localhost:80/";

        private FrontendServer()
        {
            listener = new HttpListener();
            listener.Prefixes.Add(LISTENER_PREFIX);
            resources = new ZipResourceContainer(Master.Properties.Resources.www);
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
            //always return file data
            string path = req.Url.PathAndQuery;
            if (path == "/")
                path = "/index.html";
            path = path.TrimStart('/');

            byte[] fileData;
            if (resources.TryGetValue(path, out fileData))
            {
                resp.OutputStream.Write(fileData,0,fileData.Length);
                resp.OutputStream.Close();
            }
            else
            {
                resp.StatusCode = 404;
                byte[] page404 = Page404;
                if (page404 != null)
                {
                    resp.OutputStream.Write(page404,0,page404.Length);
                }
                resp.OutputStream.Close();
            }
            /*StreamWriter sw = new StreamWriter(resp.OutputStream);
            sw.WriteLine(req.Url.PathAndQuery);
            sw.Close();*/
        }

        /// <summary>
        /// Get 404 page from resources if it exists
        /// </summary>
        private byte[] Page404
        {
            get
            {
                byte[] data;
                if (resources.TryGetValue("404.html",out data))
                {
                    return data;
                }
                return null;
            }
        }
    }
}
