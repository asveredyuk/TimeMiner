using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Frontend
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
        private IResourceContainer resources;

        private ResponseMaker responseMaker;
        public const string LISTENER_PORT = "8080";
        /// <summary>
        /// Local server prefix including port
        /// </summary>
        public const string LISTENER_PREFIX = "http://+:" + LISTENER_PORT + "/";//"http://localhost:80/";

        /// <summary>
        /// Container with files from www folder
        /// </summary>
        public IResourceContainer WWWResources
        {
            get { return resources; }
        }

        private FrontendServer()
        {
            listener = new HttpListener();
            listener.Prefixes.Add(LISTENER_PREFIX);
            resources = new DirResourceContainer("../../www");//new ZipResourceContainer(Master.Properties.Resources.www);
            responseMaker = new ResponseMaker(resources);
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
                //start task to handle this request
                Task.Run(() => GoHandleRequest(req, resp));

            }
        }
        /// <summary>
        /// Method to be called in separate thread to handle the request
        /// </summary>
        /// <param name="req"></param>
        /// <param name="resp"></param>
        private void GoHandleRequest(HttpListenerRequest req, HttpListenerResponse resp)
        {
            try
            {
                HandleRequest(req, resp);
            }
            catch (Exception e)
            {
                try
                {
                    if (resp.OutputStream.CanWrite)
                    {
                        //responce was not closed
                        //set 500 - internal server error
                        resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                        resp.Close();
                    }
                }
                catch (Exception)
                {
                    //do nothing
                    //throw;
                }
                Console.Out.WriteLine("Exception in handler thread, " + e.Message);
                Console.Out.WriteLine(e.StackTrace);
                //throw;
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
            Console.WriteLine(path);
            /*if (path == "/")
                path = "/index.html";*/
            path = path.TrimStart('/');

            byte[] fileData = resources.GetResource(path);
            if (fileData != null)
            {
                //this is resource query
                resp.OutputStream.Write(fileData,0,fileData.Length);
                resp.OutputStream.Close();
            }
            else
            {
                if (path == "api" || path.StartsWith("api/"))
                {
                    //this is api request
                    responseMaker.OnApiRequest(req,resp);
                }
                else
                {
                    //this is web interface request
                    responseMaker.OnRequest(req,resp);
                }
                /*resp.StatusCode = 404;
                byte[] page404 = Page404;
                if (page404 != null)
                {
                    resp.OutputStream.Write(page404,0,page404.Length);
                }
                resp.OutputStream.Close();*/
            }
            Console.WriteLine(path + ":A");
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
                byte[] data = resources.GetResource("404.html");
                /*if (resources.TryGetValue("404.html",out data))
                {
                    return data;
                }
                return null;*/
                return data;
            }
        }

        
    }
}
