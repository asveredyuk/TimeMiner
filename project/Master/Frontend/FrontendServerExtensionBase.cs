﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TimeMiner.Master.Frontend
{
    public abstract class FrontendServerExtensionBase
    {

        protected FrontendServerExtensionBase()
        {
        }

        protected IResourceContainer WWWRes
        {
            get { return FrontendServer.Self.WWWResources; }
        }
        /// <summary>
        /// Get the root of given path
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>String containing first root element of the path, or path if no root found</returns>
        protected string GetPathRoot(string path)
        {
            path = path.TrimStart('/');
            int pos = path.IndexOf('/');
            if (pos <= 0)
            {
                return path;
            }
            return path.Substring(0, pos);
        }
        /// <summary>
        /// Get path part without the root
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>String containing rest of part excluding root element, or "" if root not found</returns>
        protected string GetSubPath(string path)
        {
            path = path.TrimStart('/');
            string root = GetPathRoot(path);
            if (root == path)
                return "";
            return path.Substring(root.Length, path.Length - root.Length).Trim('/');
        }
        /// <summary>
        /// Remove "api/root/" from the path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected string SkipApiAndRoot(string path)
        {
            return GetSubPath(GetSubPath(path));
        }

        /// <summary>
        /// Read string of post request
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        protected string ReadPostString(HttpListenerRequest req)
        {
            //TODO: large string may make app fail!
            string str = "";
            using (StreamReader sr = new StreamReader(req.InputStream))
            {
                str = sr.ReadToEnd();
            }
            return str;
        }
        /// <summary>
        /// Write bytes to the output stream and close it
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="data"></param>
        /// <param name="code"></param>
        protected void WriteBytesAndClose(HttpListenerResponse resp, byte[] data, int code = 200)
        {
            if (code != 200)
            {
                resp.StatusCode = code;
            }
            resp.OutputStream.Write(data,0,data.Length);
            resp.OutputStream.Close();
        }

        /// <summary>
        /// Close request with code
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="code"></param>
        protected void CloseWithCode(HttpListenerResponse resp, int code)
        {
            resp.StatusCode = code;
            resp.OutputStream.Close();
        }
        /// <summary>
        /// Write string to the output stream and close it
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="str"></param>
        /// <param name="code"></param>
        protected void WriteStringAndClose(HttpListenerResponse resp, string str, int code = 200)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            WriteBytesAndClose(resp,data,code);
        }

        /// <summary>
        /// Serialize object with Newtonsoft Json, send it and close response stream
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="o"></param>
        /// <param name="code"></param>
        protected void WriteObjectJsonAndClose(HttpListenerResponse resp, object o, int code = 200)
        {
            string json = JsonConvert.SerializeObject(o, Formatting.Indented);
            WriteStringAndClose(resp,json,code);
        }
        public static string GetTokenFromRequest(HttpListenerRequest req)
        {
            var cookie = req.Cookies["auth_token"];
            if (cookie != null)
            {
                // for now, valid token is 'MasterToken';
                return cookie.Value;
            }
            //check if there is a header
            var header = req.Headers["X-Auth-Token"];
            if (header != null)
            {
                return header;
            }
            return null;
        }
    }
}
