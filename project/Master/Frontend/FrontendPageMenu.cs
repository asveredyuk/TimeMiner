using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Frontend
{
    class FrontendPageMenu
    {
        private FrontendPageMenuItem rootItem;
        public IReadOnlyList<FrontendPageMenuItem> Items => rootItem.Children;

        public FrontendPageMenu(FrontendPageMenuItem rootItem)
        {
            this.rootItem = rootItem;
        }

        public static FrontendPageMenu MakeMenu(List<FrontendPageMenuItem> items)
        {
            return FrontendPageMenuItem.MakeMenu(items);
        }

        public void Print()
        {
            foreach (var menuItem in Items)
            {
                menuItem.PrintTree();
            }
        }
    }
}
