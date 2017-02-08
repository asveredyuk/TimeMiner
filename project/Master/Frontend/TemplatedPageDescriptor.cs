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
    class TemplatedPageDescriptor
    {
        #region properties for mustache

        public string Page
        {
            get { return desc.Page; }
        }

        #endregion
        private HandlerPageDescriptor desc;

        public TemplatedPageDescriptor(HandlerPageDescriptor desc)
        {
            this.desc = desc;
        }

        
    }
}
