using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Frontend
{
    /// <summary>
    /// Descriptor of templated page
    /// </summary>
    class TemplatePageDescriptor
    {

        #region properties for mustache

        public string Page
        {
            get { return desc.Page; }
        }

        public string IncludeInHead
        {
            get { return desc.Head; }
        }

        public string TopMenu
        {
            get { return topMenu; }
        }

        public FrontendPageMenu Menu { get; private set; }
        #endregion
        private HandlerPageDescriptor desc;
        private string topMenu;

        public TemplatePageDescriptor(HandlerPageDescriptor desc, FrontendPageMenu menu, string topMenu)
        {
            this.desc = desc;
            Menu = menu;
            this.topMenu = topMenu;
        }


    }
}
