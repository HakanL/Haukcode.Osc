using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Rug.Loading
{
    public static class PluginLoader
    {
        public static void LoadPlugins(LoadContext context)
        {
            Loader.CacheLoadables(typeof(Plugin).Assembly);

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

            string pluginManifestFile = Path.Combine(path, "Plugins.xml");

            if (File.Exists(pluginManifestFile) == false)
            {
                return;
            }

            XDocument pluginManifest = XDocument.Load(pluginManifestFile);

            Plugin[] plugins = Loader.LoadObjects<Plugin>(context, pluginManifest.Root, LoaderMode.UnknownNodesError);

            context.ReportErrors();

            if (context.HasHadCriticalError == true)
            {
                return;
            }

            context.ClearErrors();

            foreach (Plugin plugin in plugins)
            {
                plugin.LoadPlugin(context, path);
            }

            context.ReportErrors();
        }
    }

    [Name("Plugin")]
    internal class Plugin : ILoadable
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public void LoadPlugin(LoadContext context, string pluginPath)
        {
            try
            {
                Assembly assemlby = Assembly.LoadFrom(System.IO.Path.Combine(pluginPath, Path));

                int count = Loader.CacheLoadables(assemlby);

                context.Reporter.PrintEmphasized($"Loaded plugin {Name} from {Path} ({count} loadable types found)");
            }
            catch (Exception ex)
            {
                context.Reporter.PrintException(ex, $"Failed to load plugin {Name} from {Path}");

                context.Error($"Failed to load plugin {Name} from {Path}", true, ex);
            }
        }

        public void Load(LoadContext context, XElement node)
        {
            Name = Helper.GetAttributeValue(node, nameof(Name), null);
            Path = Helper.GetAttributeValue(node, nameof(Path), null);

            if (string.IsNullOrEmpty(Name) == true)
            {
                context.Error($"Missing {nameof(Name)} attribute.", node);
            }

            if (string.IsNullOrEmpty(Path) == true)
            {
                context.Error($"Missing {nameof(Path)} attribute.", node);
            }
        }

        public void Save(LoadContext context, XElement element)
        {
            Helper.AppendAttributeAndValue(element, nameof(Name), Name);
            Helper.AppendAttributeAndValue(element, nameof(Path), Path);
        }
    }
}
