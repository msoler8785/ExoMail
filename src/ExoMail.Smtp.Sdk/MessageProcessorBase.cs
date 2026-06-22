using ExoMail.Smtp.Sdk;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using static ExoMail.Smtp.Sdk.PluginOptions;

namespace ExoMail.Smtp.Sdk
{ 
    public class MessageProcessorBase
    {
        private readonly PluginOption _option;

        public MessageProcessorBase(IOptions<PluginOptions> options)
        {
            if (options != null && options.Value.Plugins.TryGetValue(this.GetType().FullName, out PluginOption option))
            {
                _option = option;
            }
            else
            {
                _option = new PluginOption() { IsEnabled = false };
            }
        }

        public bool StopProcessing => _option.StopProcessing;
        public bool Enabled => _option.IsEnabled;
        public int Priority => _option.Priority;
    }
}
