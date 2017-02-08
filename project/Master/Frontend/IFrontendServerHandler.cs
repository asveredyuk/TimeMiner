using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Frontend
{
    interface IFrontendServerHandler
    {
        /// <summary>
        /// Handler request. Returns null if everything was done in handler
        /// </summary>
        /// <param name="req"></param>
        /// <param name="resp"></param>
        /// <returns>If returned, page must be embedded into main and returned</returns>
        PageBuilder Handle(HttpListenerRequest req, HttpListenerResponse resp);
    }
}
