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
        public IReadOnlyDictionary<string, OnRequestHandler> RequestHandlers
        {
            get { return _requestHandlers; }
        }

        public IReadOnlyDictionary<string, OnApiRequestHandler> ApiHanlders
        {
            get { return _apiHandlers; }
        }

        public IEnumerable<TemplatePageMenuItem> MenuItems
        {
            get { return _extensions.Select(t => t.MenuItems).Where(t => t != null).SelectMany(t => t); }
        }

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
        

        
    }
}
