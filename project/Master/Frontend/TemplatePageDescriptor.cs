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

        public FrontendPageMenu Menu { get; private set; }
        #endregion
        private HandlerPageDescriptor desc;

        public TemplatePageDescriptor(HandlerPageDescriptor desc, FrontendPageMenu menu)
        {
            this.desc = desc;
            Menu = menu;
            //Menu = new List<FrontendPageMenuItem>();
        }

        
    }
}
