using ExoMail.Smtp.Sdk;
using System;
using System.Collections.Generic;
using System.Text;
using ExoMail.Smtp.Common.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace ExoMail.Smtp.Service.Configuration
{
    public class SpamScanningProcessor : MessageProcessorBase, IMessageProcessor
    {
        private readonly IOptions<SpamScanningOptions> _spamScanningOptions;

        public SpamScanningProcessor(IOptions<PluginOptions> options, IOptions<SpamScanningOptions> spamScanningOptions) 
            : base(options)
        {
            _spamScanningOptions = spamScanningOptions;
        }

        public Task ProcessAsync(IQueuedMessage queuedMessage)
        {
            throw new NotImplementedException();
        }
    }
}
