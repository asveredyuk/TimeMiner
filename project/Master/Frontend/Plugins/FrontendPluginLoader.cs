using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Frontend.Plugins;

namespace TimeMiner.Master.Frontend
{
    class FrontendPluginLoader
    {
        #region singletone
        public static FrontendPluginLoader Self
        {
            get
            {
                if (self == null)
                {
                    self = new FrontendPluginLoader();
                }
                return self;
            }
        }
        private static FrontendPluginLoader self;
        #endregion

        public IReadOnlyDictionary<string, IFrontendServerHandler> Handlers
        {
            get { return handlers; }
        }

        private Dictionary<string, IFrontendServerHandler> handlers;
        private List<Assembly> loadedAssemblies;
        private FrontendPluginLoader()
        {
            handlers = new Dictionary<string, IFrontendServerHandler>();
            loadedAssemblies = new List<Assembly>();
            Init();
        }

        public void Init()
        {
            LoadPlugins();
            ParseHandlers();
        }
        private void ParseHandlers()
        {
            foreach (var assembly in loadedAssemblies)
            {
                foreach (var htype in GetHandlersFromAssembly(assembly))
                {
                    //TODO: check if empty constructor exitsts
                    HandlerPathAttribute[] hpathAttributes = htype.GetCustomAttributes<HandlerPathAttribute>().ToArray();
                    if (hpathAttributes.Length > 0)
                    {
                        //we have some bindings
                        object o = Activator.CreateInstance(htype);
                        IFrontendServerHandler h = (IFrontendServerHandler) o;
                        foreach (var path in hpathAttributes.Select(t=>t.path))
                        {
                            BindHandler(path,h);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Add new handler to the dictionary
        /// </summary>
        /// <param name="path"></param>
        /// <param name="handler"></param>
        private void BindHandler(string path, IFrontendServerHandler handler)
        {
            if (handlers.ContainsKey(path))
            {
                throw new Exception(string.Format("path {0} is only bound, but {1} tries to bind it again",path,handler.GetType().Name));
            }
            handlers[path] = handler;
        }

        private void LoadPlugins()
        {
            //TODO: loading plugins if feature
            loadedAssemblies.Add(Assembly.GetExecutingAssembly());//add master to list of assemblies
        }


        private IEnumerable<Type> GetHandlersFromAssembly(Assembly assembly)
        {
            return assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof (IFrontendServerHandler)));
        }
        

        
    }
}
