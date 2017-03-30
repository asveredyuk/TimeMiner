using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Frontend.Plugins;

namespace TimeMiner.Master.Frontend
{
    class FrontendExtensionLoader
    {

        #region singletone
        public static FrontendExtensionLoader Self
        {
            get
            {
                if (self == null)
                {
                    self = new FrontendExtensionLoader();
                }
                return self;
            }
        }
        private static FrontendExtensionLoader self;
        #endregion

        public delegate HandlerPageDescriptor OnRequestHandler(HttpListenerRequest req, HttpListenerResponse resp);

        public delegate void OnApiRequestHandler(HttpListenerRequest req, HttpListenerResponse resp);

        public IReadOnlyList<FrontendServerExtensionBase> Extensions
        {
            get { return _extensions; }
        }

        public FrontendPageMenu Menu { get; private set; }

        private List<FrontendServerExtensionBase> _extensions;
        private Dictionary<string, OnRequestHandler> _requestHandlers;
        private Dictionary<string, OnApiRequestHandler> _apiHandlers;
        private List<Assembly> loadedAssemblies;
        private FrontendExtensionLoader()
        {
            _extensions = new List<FrontendServerExtensionBase>();
            loadedAssemblies = new List<Assembly>();
            _requestHandlers = new Dictionary<string, OnRequestHandler>();
            _apiHandlers = new Dictionary<string, OnApiRequestHandler>();
            Init();
        }

        public void Init()
        {
            LoadPlugins();
            ParseExtensions();
            ParseHandlers();
            ParseMenu();
        }

        public OnRequestHandler GetRequestHandler(string path)
        {
            return GetItemFromDictWithPath(path, _requestHandlers);
        }

        public OnApiRequestHandler GetApiRequestHandler(string path)
        {
            return GetItemFromDictWithPath(path, _apiHandlers);
        }

        private static T GetItemFromDictWithPath<T>(string path, Dictionary<string, T> dict) where T:class
        {
            T result;
            while (true)
            {
                if (dict.TryGetValue(path, out result))
                {
                    return result;
                }
                if (path.Contains("/"))
                {
                    path = path.Substring(0, path.LastIndexOf('/'));
                }
                else
                {
                    break;
                }
            }
            return null;
        }
        /// <summary>
        /// Load plugins assemblies
        /// </summary>
        private void LoadPlugins()
        {
            //TODO: loading plugins if feature
            loadedAssemblies.Add(Assembly.GetExecutingAssembly());//add master to list of assemblies
        }
        /// <summary>
        /// Parse extensions from loaded assemblies
        /// </summary>
        private void ParseExtensions()
        {
            foreach (var assembly in loadedAssemblies)
            {
                foreach (var exType in GetHandlersFromAssembly(assembly))
                {
                    FrontendServerExtensionBase ex = (FrontendServerExtensionBase) Activator.CreateInstance(exType);
                    _extensions.Add(ex);
                }
            }
        }
        /// <summary>
        /// Parse handlers from added extensions
        /// </summary>
        private void ParseHandlers()
        {
            foreach (var ex in _extensions)
            {
                Type t = ex.GetType();
                MethodInfo[] allMethods = t.GetMethods(BindingFlags.Instance|BindingFlags.Public);
                foreach (var methodInfo in allMethods)
                {
                    HandlerPathAttribute[] attrs = methodInfo.GetCustomAttributes<HandlerPathAttribute>().ToArray();
                    if (attrs.Length > 0)
                    {
                        //cast method to delegate
                        OnRequestHandler h = Delegate.CreateDelegate(typeof (OnRequestHandler), ex, methodInfo) as OnRequestHandler;
                        if (h != null)
                        {
                            foreach (var attr in attrs)
                            {
                                AddHandler(attr.path,h);
                            }
                        }
                        else
                        {
                            Console.Out.WriteLine("{0}.{1} does not fit delegate",t.Name,methodInfo.Name);
                        }
                    }
                    ApiPathAttribute[] apiAttrs = methodInfo.GetCustomAttributes<ApiPathAttribute>().ToArray();
                    if (apiAttrs.Length > 0)
                    {
                        //cast method to delegate
                        OnApiRequestHandler h = Delegate.CreateDelegate(typeof(OnApiRequestHandler), ex, methodInfo) as OnApiRequestHandler;
                        if (h != null)
                        {
                            foreach (var attr in apiAttrs)
                            {
                                AddApiHandler(attr.path, h);
                            }
                        }
                        else
                        {
                            Console.Out.WriteLine("{0}.{1} does not fit delegate", t.Name, methodInfo.Name);
                        }
                    }
                }

            }
        }
        /// <summary>
        /// Add handler to handlers dictionary, if key is not already occupied
        /// </summary>
        /// <param name="key"></param>
        /// <param name="h"></param>
        private void AddHandler(string key, OnRequestHandler h)
        {
            if (key == "api")
            {
                Console.Out.WriteLine("api key cannot be occupied");
                return;
            }
            if (_requestHandlers.ContainsKey(key))
            {
                Console.Out.WriteLine("{0} is already occupied",key);
                return;
            }
            _requestHandlers[key] = h;
        }
        /// <summary>
        /// Add api handler to api handlers dictionary, if key is not already occupied
        /// </summary>
        /// <param name="key"></param>
        /// <param name="h"></param>
        private void AddApiHandler(string key, OnApiRequestHandler h)
        {
            if (_apiHandlers.ContainsKey(key))
            {
                Console.Out.WriteLine("{0} is already occupied", key);
                return;
            }
            _apiHandlers[key] = h;
        }

        /// <summary>
        /// Get all types from assembly, that implements IFrontendServerExtension
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private IEnumerable<Type> GetHandlersFromAssembly(Assembly assembly)
        {
            return assembly.GetTypes()
                //only non abstract non nested classes can be extensions
                .Where(t=>t.IsClass&&!t.IsAbstract&&!t.IsNested)
                //it must implement interface
                .Where(t => t.IsSubclassOf(typeof(FrontendServerExtensionBase)))
                //it must have public constructor with no arguments
                .Where(t =>
                {
                    ConstructorInfo constr = t.GetConstructor(Type.EmptyTypes);
                    return constr != null && constr.IsPublic;
                });
        }

        private void ParseMenu()
        {
            List<FrontendPageMenuItem> items = ParseMenuItems(_extensions.Select(t => t.GetType()).ToArray());
            Menu = FrontendPageMenu.MakeMenu(items);
        }
        private List<FrontendPageMenuItem> ParseMenuItems(Type[] types)
        {
            List<FrontendPageMenuItem> menuItems = new List<FrontendPageMenuItem>();
            foreach (var type in types)
            {
                IEnumerable<MenuItemAttribute> clAttrs = type.GetCustomAttributes<MenuItemAttribute>();
                foreach (var clAttr in clAttrs)
                {
                    var item = new FrontendPageMenuItem(clAttr.label, clAttr.url, clAttr.menuPath, clAttr.order);
                    menuItems.Add(item);
                }
                foreach (var methodInfo in type.GetMethods())
                {
                    MenuItemAttribute[] menuAttributes = methodInfo.GetCustomAttributes<MenuItemAttribute>().ToArray();
                    if (menuAttributes.Length == 0)
                    {
                        continue;
                    }
                    if (menuAttributes.Length > 1)
                    {
                        Console.WriteLine("Menu warning: more than one menu handler on method " + type.Name + "." + methodInfo.Name + " , first used");
                    }
                    MenuItemAttribute mattr = menuAttributes.First();
                    string url = mattr.url;
                    if (url == null)
                    {
                        HandlerPathAttribute[] pathAttributes = methodInfo.GetCustomAttributes<HandlerPathAttribute>().ToArray();
                        if (pathAttributes.Length == 0)
                        {
                            Console.WriteLine("Menu warning: no path found for menu for method " + type.Name + "." +
                                              methodInfo.Name);
                        }
                        else
                        {
                            if (pathAttributes.Length > 1)
                            {
                                Console.WriteLine("Menu warning: more than one path found in method " + type.Name + "." +
                                              methodInfo.Name);
                            }
                            HandlerPathAttribute pattr = pathAttributes.First();
                            url = pattr.path;
                        }
                    }
                    var item = new FrontendPageMenuItem(mattr.label, url, mattr.menuPath, mattr.order);
                    menuItems.Add(item);
                }
            }
            return menuItems;
        }



    }
}
