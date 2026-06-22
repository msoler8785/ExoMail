using ExoMail.Smtp.Client.Network;
using ExoMail.Smtp.Common.Enums;
using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Common.Models;
using ExoMail.Smtp.Server.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ExoMail.Smtp.QueueProcessor
{
    public class CachedSmtpClient
    {
        private readonly ExecutionDataflowBlockOptions _blockOptions;
        private readonly string _domain;
        private readonly IMessageEnvelopeRepository _envelopeRepository;
        private readonly IGatewayOptions _gatewayOptions;
        private readonly IPropagatorBlock<IQueuedMessage, IQueuedMessage> _queue;
        private readonly SmtpClient _smtpClient;
        private ActionBlock<IQueuedMessage> _block;
        public bool Expired { get; private set; }
        public int SentMessages { get; private set; } = 0;

        //seconds
        public CachedSmtpClient(
            string domain,
            IPropagatorBlock<IQueuedMessage, IQueuedMessage> queue,
            SmtpClient smtpClient,
            IMessageEnvelopeRepository envelopeRepository,
            IGatewayOptions gatewayOptions
            )
        {
            _domain = domain;
            _queue = queue;
            _smtpClient = smtpClient;
            _envelopeRepository = envelopeRepository;
            _gatewayOptions = gatewayOptions;
            _blockOptions = new ExecutionDataflowBlockOptions() { BoundedCapacity = 1, MaxDegreeOfParallelism = 1 };
        }

        public void Initialize()
        {
            _block = new ActionBlock<IQueuedMessage>(async (message) =>
            {
                var envelopes = message.Envelopes
                      .Where(x => x.DeliveryRetryable)
                      .Where(x => x.Domain.Equals(_domain, StringComparison.OrdinalIgnoreCase))
                      //.Where(x => x.NextAttempt <= DateTime.Now)
                      .ToList();

                if (envelopes.Any())
                {
                    using (var stream = await message.MessageData.GetMessagStreamAsync())
                    {
                        await ProcessMessage(stream, message.Sender, envelopes, message?.DkimPolicy);
                    }
                }

                if (envelopes.Any(x => !x.DeliveryComplete))
                {
                    await _queue.SendAsync(message);
                }

                if (message.DeliveryComplete)
                {
                    message.MessageData.DeleteMessageData();
                }
            },
            _blockOptions);
            _queue.LinkTo(_block);
        }

        private async Task ProcessMessage(Stream message, MailAddress sender, List<IMessageEnvelope> envelopes, IDkimPolicy dkimPolicy)
        {
            try
            {
                await _smtpClient.SendAsync(sender.FullAddress, envelopes, message, dkimPolicy);
            }
            catch (Exception ex)
            {
                Complete();

                envelopes.ForEach(x =>
                {
                    x.LastResponse = ex.Message;
                    x.Disposition = Disposition.TempFail;
                });
            }
            finally
            {
                envelopes.ForEach(async x =>
                {
                    x.DeliveryAttempts++;

                    if (x.DeliveryAttempts > _gatewayOptions.MaxMessageRetries)
                    {
                        x.Disposition = Disposition.PermFail;
                        x.LastResponse = SmtpResponse.MessageExpired;
                    }

                    if (x.DeliveryRetryable)
                        x.NextAttempt = DateTime.Now.AddSeconds(_gatewayOptions.MessageDeferralTime);

                    _envelopeRepository.Update(x);

                    if (x.DeliveryComplete)
                        await _envelopeRepository.ArchiveAsync(x);
                });

                if (SentMessages > _gatewayOptions.MaxMessagesPerSession)
                {
                    Complete();
                }
            }

            SentMessages++;
        }

        private void Complete()
        {
            Expired = true;
            _block.Complete();

            if (_smtpClient.IsConnected)
                _smtpClient.Dispose();
        }
    }
}