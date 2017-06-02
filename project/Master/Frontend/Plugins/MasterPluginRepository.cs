using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using TimeMiner.Core.Plugging;

namespace TimeMiner.Master.Frontend.Plugins
{
    public class MasterPluginRepository : PluginRepository
    {
        /// <summary>
        /// Lock, to prevent multiple initialization
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static MasterPluginRepository self;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static MasterPluginRepository Self
        {
            get
            {
                lock (_lock)
                {
                    if (self == null)
                    {
                        self = new MasterPluginRepository();
                    }
                    return self;
                }
            }
        }

        protected override IEnumerable<Assembly> GetAdditionalAssemblies()
        {
            //Master assembly
            yield return Assembly.GetExecutingAssembly();
        }
        /// <summary>
        /// Get bytes of given assembly
        /// </summary>
        /// <param name="ass"></param>
        /// <returns>Byte array or null</returns>
        public byte[] GetAssemblyBytes(Assembly ass)
        {
            lock (_lock)
            {
                if (!assembly2fname.ContainsKey(ass))
                {
                    return null;
                }
                string path = PLUGIN_REPO + "/" + assembly2fname[ass];
                byte[] data = File.ReadAllBytes(path);
                return data;
            }
        }
        /// <summary>
        /// Get bytes of assembly with given guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>Byte array or null</returns>
        public byte[] GetAssemblyBytes(Guid guid)
        {
            string guidStr = guid.ToString();
            var ass = assemblies.FirstOrDefault(t => t.GetCustomAttribute<GuidAttribute>().Value == guidStr);
            if (ass == null)
                return null;
            return GetAssemblyBytes(ass);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IEnumerable<object> GetObjectsToDestroy(IEnumerable<object> objects, Assembly[] assemblies)
        {
            foreach (var o in objects)
            {
                if (assemblies.All(t => o.GetType().Assembly != t))
                {
                    //no assembly for given object
                    yield return o;
                }
            }
        }

        public static IEnumerable<Type> GetNonExistingObjects(object[] objects, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                if (objects.All(t => t.GetType() != type))
                {
                    yield return type;
                }
            }
        }
    }
}
