using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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

        /// <summary>
        /// Delegate to handle page
        /// </summary>
        /// <param name="req">Listener request</param>
        /// <param name="resp">Listener response</param>
        /// <returns></returns>
        public delegate HandlerPageDescriptor OnRequestHandler(HttpListenerRequest req, HttpListenerResponse resp);

        /// <summary>
        /// Delegate to handle api call
        /// </summary>
        /// <param name="req">Listener request</param>
        /// <param name="resp">Listener response</param>
        public delegate void OnApiRequestHandler(HttpListenerRequest req, HttpListenerResponse resp);

        public class HandlerMethodDescriptor<T>
        {
            public T Handler { get; }
            public bool IsPublic { get; set; }

            public HandlerMethodDescriptor(T handler)
            {
                Handler = handler;
            }
        }
        /// <summary>
        /// List of loaded extensions
        /// </summary>
        public IReadOnlyList<FrontendServerExtensionBase> Extensions
        {
            get { return _extensions; }
        }
        /// <summary>
        /// Menu, parsed from loaded extensions
        /// </summary>
        public FrontendPageMenu Menu { get; private set; }
        /// <summary>
        /// List of extensions
        /// </summary>
        private List<FrontendServerExtensionBase> _extensions;
        /// <summary>
        /// Dictinary [path]->[request handler]
        /// </summary>
        private Dictionary<string, HandlerMethodDescriptor<OnRequestHandler>> _requestHandlers;
        /// <summary>
        /// Dictionary [path]->[api handler]
        /// </summary>
        private Dictionary<string, HandlerMethodDescriptor<OnApiRequestHandler>> _apiHandlers;
        /// <summary>
        /// List of loaded assemblies
        /// </summary>
        private List<Assembly> loadedAssemblies;
        private FrontendExtensionLoader()
        {
            _extensions = new List<FrontendServerExtensionBase>();
            loadedAssemblies = new List<Assembly>();
            _requestHandlers = new Dictionary<string, HandlerMethodDescriptor<OnRequestHandler>>();
            _apiHandlers = new Dictionary<string, HandlerMethodDescriptor<OnApiRequestHandler>>();
            Init();
        }
        /// <summary>
        /// Initialize loader
        /// </summary>
        public void Init()
        {
            LoadPlugins();
            ParseExtensions();
            ParseHandlers();
            ParseMenu();
        }
        /// <summary>
        /// Get handler for given request path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Handler or null</returns>
        public HandlerMethodDescriptor<OnRequestHandler> GetRequestHandler(string path)
        {
            return GetItemFromDictWithPath(path, _requestHandlers);
        }
        /// <summary>
        /// Get handler for given api request path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public HandlerMethodDescriptor<OnApiRequestHandler> GetApiRequestHandler(string path)
        {
            return GetItemFromDictWithPath(path, _apiHandlers);
        }
        /// <summary>
        /// Get the most appropriate item from given dicionary for given path
        /// </summary>
        /// <typeparam name="T">Type of items</typeparam>
        /// <param name="path">Path</param>
        /// <param name="dict">Dictionary with items</param>
        /// <returns>Item if found and null if not</returns>
        private static T GetItemFromDictWithPath<T>(string path, Dictionary<string, T> dict) where T:class
        {
            T result;
            //slowly downgrade to the root
            //example:
            //we have path ext/a/b
            //the order of checks
            //ext/a/b
            //ext/a
            //ext
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
                                var desc = new HandlerMethodDescriptor<OnRequestHandler>(h);
                                ParseExtraParamsForMethod(desc, methodInfo);
                                AddHandler(attr.path,desc);
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
                                var desc = new HandlerMethodDescriptor<OnApiRequestHandler>(h);
                                ParseExtraParamsForMethod(desc, methodInfo);
                                AddApiHandler(attr.path, desc);
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

        private void ParseExtraParamsForMethod<T>(HandlerMethodDescriptor<T> descriptor, MethodInfo info)
        {
            if (info.GetCustomAttributes<PublicHandlerAttribute>().Any())
            {
                descriptor.IsPublic = true;
            }
        }
        /// <summary>
        /// Add handler to handlers dictionary, if key is not already occupied
        /// </summary>
        /// <param name="key"></param>
        /// <param name="h"></param>
        private void AddHandler(string key, HandlerMethodDescriptor<OnRequestHandler> h)
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
        private void AddApiHandler(string key, HandlerMethodDescriptor<OnApiRequestHandler> h)
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
        /// <summary>
        /// Parse menu from loaded extensions
        /// </summary>
        private void ParseMenu()
        {
            List<FrontendPageMenuItem> items = ParseMenuItems(_extensions.Select(t => t.GetType()).ToArray());
            Menu = FrontendPageMenu.MakeMenu(items);
        }
        /// <summary>
        /// Parse menuitems from given types
        /// </summary>
        /// <param name="types">Types of loaded extensions</param>
        /// <returns></returns>
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
