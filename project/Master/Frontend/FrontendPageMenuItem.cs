using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Frontend
{
    class FrontendPageMenuItem
    {
        /// <summary>
        /// Default value of order
        /// </summary>
        public const int DEFAULT_ORDER = 10;
        /// <summary>
        /// Label of menu item
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Link of given label
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Path in menu tree
        /// </summary>
        private string menuPath;
        /// <summary>
        /// Relative order
        /// </summary>
        private int order;
        /// <summary>
        /// Dictionary [name]->[item]
        /// </summary>
        private Dictionary<string, FrontendPageMenuItem> children;
        /// <summary>
        /// List of children of given item, ordered
        /// </summary>
        public IReadOnlyList<FrontendPageMenuItem> Children
        {
            get { return children.Select(t => t.Value).OrderBy(t => t.order).ToList(); }
        }
        /// <summary>
        /// Get or set children via its name
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public FrontendPageMenuItem this[string key]
        {
            get
            {
                FrontendPageMenuItem ite;
                children.TryGetValue(key, out ite);
                return ite;
            }
            set { children[key] = value; }
        }
        /// <summary>
        /// Create new empty menu item
        /// </summary>
        public FrontendPageMenuItem()
        {
            children = new Dictionary<string, FrontendPageMenuItem>();
        }
        /// <summary>
        /// Create new menu item
        /// </summary>
        /// <param name="label"></param>
        /// <param name="url"></param>
        /// <param name="menuPath"></param>
        /// <param name="order"></param>
        public FrontendPageMenuItem(string label, string url, string menuPath, int order = DEFAULT_ORDER) : this()
        {
            Label = label;
            Url = url;
            this.menuPath = menuPath;
            this.order = order;
        }
        /// <summary>
        /// Create menu from given items
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static FrontendPageMenu MakeMenu(List<FrontendPageMenuItem> items)
        {
            FrontendPageMenuItem root = new FrontendPageMenuItem();
            foreach (var item in items.OrderBy(t => GetNestingLevel(t.menuPath)))
            {
                PutItemToTree(root, item);
            }
            return new FrontendPageMenu(root);
        }
        /// <summary>
        /// Get nesting level of item with given menu path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int GetNestingLevel(string path)
        {
            return path.Count(t => t == '/');
        }
        /// <summary>
        /// Convert path to queue of keys
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static Queue<string> GetPathChain(string path)
        {
            string[] arr = path.Split('/');
            return new Queue<string>(arr);
        }
        /// <summary>
        /// Put given to the tree with given root
        /// </summary>
        /// <param name="root">The root of tree</param>
        /// <param name="item">Item to put</param>
        public static void PutItemToTree(FrontendPageMenuItem root, FrontendPageMenuItem item)
        {
            Queue<string> chain = GetPathChain(item.menuPath);
            FrontendPageMenuItem parent = root;
            while (chain.Count != 1)
            {
                string nowStr = chain.Dequeue();
                FrontendPageMenuItem now = parent[nowStr];
                if (now == null)
                {
                    string nowPath = parent.menuPath;
                    LogWarning($"no item {nowStr} found in {nowPath}, created automatically");
                    //create new element
                    now = new FrontendPageMenuItem(nowStr, null, (nowPath + "/" + nowStr).TrimStart('/'));
                    parent[nowStr] = now;
                }
                parent = now;
            }
            string givenitemStr = chain.Dequeue();
            parent[givenitemStr] = item;
        }
        /// <summary>
        /// Print to console warning
        /// </summary>
        /// <param name="message"></param>
        private static void LogWarning(string message)
        {
            Console.WriteLine("Warning in menu: " + message);
        }
        /// <summary>
        /// Print tree to console (for debug)
        /// </summary>
        public void PrintTree()
        {
            int nestingLevel = GetNestingLevel(this.menuPath);
            string indent = new string(' ', nestingLevel * 2);
            Console.WriteLine(indent + this.Label + "\t\t\t" + this.Url);
            foreach (var menuItem in Children)
            {
                menuItem.PrintTree();
            }
        }
    }
}
