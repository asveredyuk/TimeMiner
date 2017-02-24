using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Frontend
{
    abstract class FrontendServerExtensionBase
    {
        public List<TemplatePageMenuItem> MenuItems { get; protected set; }

        protected FrontendServerExtensionBase()
        {
            MenuItems = new List<TemplatePageMenuItem>();
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
    }
}
