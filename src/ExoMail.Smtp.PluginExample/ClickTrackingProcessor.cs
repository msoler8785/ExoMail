using ExoMail.Smtp.Sdk;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ExoMail.Smtp.Common.Interfaces;
using System.Threading.Tasks;
using System.Net.Mime;
using MimeKit;
using Microsoft.Extensions.Options;
using static ExoMail.Smtp.Sdk.PluginOptions;

namespace ExoMail.Smtp.PluginExample
{
    public class ClickTrackingProcessor : IMessageProcessor
    {
        private readonly PluginOption _option;

        public bool StopProcessing => _option.StopProcessing;
        public bool Enabled => _option.IsEnabled;
        public int Priority => _option.Priority;

        public ClickTrackingProcessor(IOptions<PluginOptions> options)
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

        public async Task ProcessAsync(IQueuedMessage queuedMessage)
        {
            if (queuedMessage.VirtualServer.Options.TryGetValue("TrackingDomain", out string trackingDomain))
            {
                var bodyParts = queuedMessage.MessageData.MimeMesage
                    .BodyParts.OfType<TextPart>().Where(x => x.IsHtml);

                foreach (var part in bodyParts)
                {
                    part.Text = part.Text
                        .Replace("href=\"", $"href=\"http://{trackingDomain}/{queuedMessage.Envelopes.FirstOrDefault().MessageId}&url=");
                }

                await queuedMessage.MessageData.SaveChangesAsync();
            }
        }
    }
}
