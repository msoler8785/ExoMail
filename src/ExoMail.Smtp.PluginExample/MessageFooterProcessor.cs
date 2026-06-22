using ExoMail.Smtp.Sdk;
using System;
using System.Collections.Generic;
using System.Text;
using ExoMail.Smtp.Common.Interfaces;
using System.Threading.Tasks;
using static ExoMail.Smtp.Sdk.PluginOptions;
using Microsoft.Extensions.Options;
using System.Linq;
using MimeKit;

namespace ExoMail.Smtp.PluginExample
{
    public class MessageFooterProcessor : MessageProcessorBase, IMessageProcessor
    {


        public MessageFooterProcessor(IOptions<PluginOptions> options)
            : base(options)
        {
        }

        public async Task ProcessAsync(IQueuedMessage queuedMessage)
        {
            if (queuedMessage.VirtualServer.Options.TryGetValue("MessageFooter", out string messageFooter))
            {
                var bodyParts = queuedMessage.MessageData.MimeMesage
                    .BodyParts.OfType<TextPart>();

                foreach (var part in bodyParts)
                {
                    var sb = new StringBuilder(part.Text);

                    if (part.IsHtml)
                    {
                        sb.Append("<p>");
                        sb.Append(messageFooter);
                        sb.Append("</p>");
                    }
                    else
                    {
                        sb.AppendLine();
                        sb.AppendLine(messageFooter);
                    }

                    part.Text = sb.ToString();
                }

                await queuedMessage.MessageData.SaveChangesAsync();
            }
        }
    }
}
