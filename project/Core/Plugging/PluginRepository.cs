using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Core.Plugging
{
    public class PluginRepository
    {
        public const string PLUGIN_REPO = "plugins";
        public delegate void OnAssembliesChangedHandler();
        /// <summary>
        /// Lock, to prevent multiple initialization
        /// </summary>
        protected readonly object _lock = new object();

        public event OnAssembliesChangedHandler onAssembliesChanged;
        /// <summary>
        /// Raise onAssembliesChanged event
        /// </summary>
        protected void RaiseEvent()
        {
            if (onAssembliesChanged != null)
                onAssembliesChanged();
        }
        /// <summary>
        /// Map from assembly to file name
        /// </summary>
        protected Dictionary<Assembly, string> assembly2fname;
        /// <summary>
        /// List of plugin assemblies
        /// </summary>
        protected List<Assembly> assemblies;
        /// <summary>
        /// List of additional assemblies
        /// </summary>
        protected List<Assembly> additionalAssemblies;

        public PluginRepository()
        {
            assemblies = new List<Assembly>();
            additionalAssemblies = new List<Assembly>();
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
                //add given additional assemblies (ex. exe) to the assemblies list
                additionalAssemblies = GetAdditionalAssemblies().ToList();
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
        /// <summary>
        /// Get Plugin descriptors
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<Assembly, PluginDescriptor>> GetDescriptors()
        {
            lock (_lock)
            {
                foreach (var assembly in assemblies)
                {
                    var guid = Guid.Parse(assembly.GetCustomAttribute<GuidAttribute>().Value);
                    var version = assembly.GetName().Version;
                    var descriptor = new PluginDescriptor(guid,version);
                    yield return new KeyValuePair<Assembly, PluginDescriptor>(assembly,descriptor);
                }
            }
        }
        /// <summary>
        /// Get additional assemblies, for example - exe assembly
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<Assembly> GetAdditionalAssemblies()
        {
            yield break;
        }
        /// <summary>
        /// Get types, derived from given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<Type> GetInstantiatableTypesDerivedFrom(Type type)
        {
            lock (_lock)
            {
                return GetValidAssemblies().SelectMany(t=>t.GetTypes())
                    //only non abstract non nested classes can be extensions
                    .Where(t => t.IsClass && !t.IsAbstract && !t.IsNested)
                    //it must implement interface
                    .Where(t => t.IsSubclassOf(type))
                    //it must have public constructor with no arguments
                    .Where(t =>
                    {
                        ConstructorInfo constr = t.GetConstructor(Type.EmptyTypes);
                        return constr != null && constr.IsPublic;
                    });
            }
        }
        /// <summary>
        /// Get types, derived from given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<Type> GetInstantiatableTypesDerivedFrom<T>() where T : class
        {
            return GetInstantiatableTypesDerivedFrom(typeof(T));
        }
        /// <summary>
        /// Install assembly and raise event
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Uninstall assembly and raise event
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Remove assembly from the repository
        /// </summary>
        /// <param name="ass"></param>
        protected void RemoveAssembly(Assembly ass)
        {
            lock (_lock)
            {
                assemblies.Remove(ass);
                string fname = assembly2fname[ass];
                //TODO:extract to separate method
                File.Delete(PLUGIN_REPO + "/" + fname);
                assembly2fname.Remove(ass);
            }
        }
        /// <summary>
        /// Get list of all assemblies (valid and not)
        /// </summary>
        /// <returns></returns>
        public List<Assembly> GetAllAssemblies()
        {
            lock (_lock)
            {
                var list = new List<Assembly>();
                list.AddRange(additionalAssemblies);
                list.AddRange(assemblies);
                return list;
            }
        }

        /// <summary>
        /// Get assemblies that are properly loaded
        /// </summary>
        /// <returns></returns>
        public List<Assembly> GetValidAssemblies()
        {
            //TODO: redo, VERY VERY VERY bad code
            List<Assembly> good = new List<Assembly>();
            foreach (var ass in GetAllAssemblies())
            {
                try
                {
                    var types = ass.GetTypes();
                    good.Add(ass);
                }
                catch (Exception e)
                {
                    //this assembly cannot be loaded
                    //for example - additional references
                }
            }
            return good;
        }
    }
}
