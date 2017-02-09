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
        public FrontendServerExtensionBase()
        {
            MenuItems = new List<TemplatePageMenuItem>();
        }
    }
}
