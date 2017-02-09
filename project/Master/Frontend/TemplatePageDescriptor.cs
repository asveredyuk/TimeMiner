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

        public List<TemplatePageMenuItem> Menu { get; private set; }
        #endregion
        private HandlerPageDescriptor desc;

        public TemplatePageDescriptor(HandlerPageDescriptor desc)
        {
            this.desc = desc;
            Menu = new List<TemplatePageMenuItem>();
        }

        
    }
}
