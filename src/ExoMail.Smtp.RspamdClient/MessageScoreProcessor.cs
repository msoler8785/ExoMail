using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Sdk;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExoMail.Smtp.RspamdClient
{
    public class MessageScoreProcessor : MessageProcessorBase, IMessageProcessor
    {
        private readonly RspamdClient _rspamdClient;

        public MessageScoreProcessor(IOptions<PluginOptions> options, IOptions<SpamScanningOptions> spamScanningOptions) 
            : base(options)
        {
            if (spamScanningOptions?.Value != null)
            {
                _rspamdClient = new RspamdClient(spamScanningOptions.Value.RspamdUri);
            }
        }

        public async Task ProcessAsync(IQueuedMessage queuedMessage)
        {

            var results = await _rspamdClient.ScoreMessageAsync(queuedMessage);

            if (results.DefaultMetric.IsSpam)
            {
                queuedMessage.MessageData
                    .MimeMesage
                    .Subject = queuedMessage.MessageData
                    .MimeMesage
                    .Subject.Insert(0, $"[SPAM] ({results.DefaultMetric.Score}) ");

                var symbols = results.DefaultMetric.Symbols.Select(x => x.ToString());
                string header = String.Concat(symbols);

                queuedMessage.MessageData.MimeMesage.Headers.Add("X-SpamdResult", header);
                await queuedMessage.MessageData.SaveChangesAsync();
            }
        }
    }
}
