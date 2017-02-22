using System;
using System.Collections.Generic;
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
            return path.Substring(root.Length, path.Length - root.Length);
        }
    }
}
