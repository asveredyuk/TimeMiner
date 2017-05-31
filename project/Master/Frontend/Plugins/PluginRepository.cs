using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace TimeMiner.Master.Frontend.Plugins
{
    public class PluginRepository
    {
        public const string PLUGIN_REPO = "plugins";
        public delegate void OnAssembliesChangedHandler(List<Assembly> assemblies);
        /// <summary>
        /// Lock, to prevent multiple initialization
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static PluginRepository self;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static PluginRepository Self
        {
            get
            {
                lock (_lock)
                {
                    if (self == null)
                    {
                        self = new PluginRepository();
                    }
                    return self;
                }
            }
        }

        public event OnAssembliesChangedHandler onAssembliesChanged;

        private void RaiseEvent()
        {
            if (onAssembliesChanged != null)
                onAssembliesChanged(assemblies);
        }
        private Dictionary<Assembly, string> assembly2fname;
        private List<Assembly> assemblies;

        private PluginRepository()
        {
            assemblies = new List<Assembly>();
            assembly2fname = new Dictionary<Assembly, string>();
        }

        /// <summary>
        /// Load plugins from the plugin repository
        /// </summary>
        public void Init()
        {
            lock (_lock)
            {
                if (!Directory.Exists(PLUGIN_REPO))
                    Directory.CreateDirectory(PLUGIN_REPO);
                //add given exe to the assemblies list
                assemblies.Add(Assembly.GetExecutingAssembly());
                assembly2fname[Assembly.GetExecutingAssembly()] = "";
                string[] files = Directory.GetFiles(PLUGIN_REPO, "*.dll");
                foreach (var file in files)
                {
                    try
                    {
                        byte[] data = File.ReadAllBytes(file);
                        Assembly assembly = Assembly.Load(data);
                        //ok
                        assemblies.Add(assembly);
                        assembly2fname[assembly] = Path.GetFileName(file);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to load file {0} as plugin" + file);
                        Console.WriteLine(e);
                    }
                }
                //initialization event
                RaiseEvent();
            }
            
        }

        public bool TryInstallAssembly(byte[] data)
        {
            lock (_lock)
            {
                try
                {
                    var assembly = Assembly.Load(data);
                    var guid = assembly.GetCustomAttribute<GuidAttribute>()?.Value;
                    if (guid == null)
                    {
                        //TODO: log that assembly should have attr
                        Console.WriteLine("Tried to install assembly without guid, {0}", assembly.FullName);
                        return false;
                    }
                    var analog =
                        assemblies.FirstOrDefault(t => t.GetCustomAttribute<GuidAttribute>()?.Value == guid);
                    if (analog != null)
                    {
                        RemoveAssembly(analog);
                    }
                    //install assembly
                    string fname = guid + ".dll";
                    File.WriteAllBytes(PLUGIN_REPO + "/" + fname, data);
                    assemblies.Add(assembly);
                    assembly2fname[assembly] = fname;
                    //notify everyone, new plugin!
                    RaiseEvent();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }

        }

        public bool TryUninstallAssembly(Guid guid)
        {
            lock (_lock)
            {
                string str = guid.ToString();
                var candidate = assemblies.FirstOrDefault(t => t.GetCustomAttribute<GuidAttribute>()?.Value == str);
                if (candidate == null)
                    return false;
                RemoveAssembly(candidate);
                RaiseEvent();
                return true;
            }

        }
        private void RemoveAssembly(Assembly ass)
        {
            lock (_lock)
            {
                assemblies.Remove(ass);
                string fname = assembly2fname[ass];
                File.Delete(PLUGIN_REPO + "/" + fname);
                assembly2fname.Remove(ass);
            }

        }
        public List<Assembly> GetAllAssemblies()
        {
            lock (_lock)
            {
                return assemblies;
            }
        }

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
