using ExoMail.Smtp.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExoMail.Smtp.Sdk
{
    public class PluginOptions
    {
        public Dictionary<string, PluginOption> Plugins { get; set; }

        public PluginOptions()
        {
            Plugins = new Dictionary<string, PluginOption>();
        }

 
        public bool IsEnabled(object plugin)
        {
            var name = plugin.GetType().FullName;

            Plugins.TryGetValue(name, out PluginOption pluginOption);

            return pluginOption != null && pluginOption.IsEnabled;
        }

        public class PluginOption
        {
            public bool StopProcessing { get; set; }
            public bool IsEnabled { get; set; }
            public int Priority { get; set; }
        }
    }
}