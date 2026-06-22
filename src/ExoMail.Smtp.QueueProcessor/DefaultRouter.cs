using ExoMail.Smtp.Common.Enums;
using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Sdk;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace ExoMail.Smtp.QueueProcessor
{
    public class DefaultRouter : IDefaultRouter
    {
        private readonly ILogger<DefaultRouter> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMessageEnvelopeRepository _envelopeRepository;
        private readonly ISmtpClientResolver _smtpClientResolver;
        private readonly IVirtualServerRepository _serverRepository;
        private readonly SemaphoreSlim _semaphore;

        public DefaultRouter(
            ILoggerFactory loggerFactory,
            IMessageEnvelopeRepository envelopeRepository,
            ISmtpClientResolver smtpClientResolver,
            IVirtualServerRepository serverRepository
            )
        {
            _logger = loggerFactory.CreateLogger<DefaultRouter>();
            _loggerFactory = loggerFactory;
            _envelopeRepository = envelopeRepository;
            _smtpClientResolver = smtpClientResolver;
            _serverRepository = serverRepository;
            _semaphore = new SemaphoreSlim(1);
        }

        public int Priority => int.MinValue;

        public bool StopRouting => true;

        public async Task RouteAsync(IQueuedMessage queuedMessage)
        {
            _logger.LogDebug("Routing QueuedMessage: {0}", queuedMessage);
            bool deliveryComplete = ProcessLocalRecipients(queuedMessage);

            if (!deliveryComplete)
            {
                var domains = queuedMessage.Envelopes.GroupBy(x => x.Domain);
                foreach (var domain in domains)
                {
                    IMessageRoute route = queuedMessage.VirtualServer
                                            .Routes
                                            .OrderBy(x => x.Metric)
                                            .FirstOrDefault(x => x.IsMatch(domain.Key));

                    _logger.LogDebug("Enqueuing QueuedMessage: {0} for {1}", queuedMessage, domain.Key);
                    await route.EnqueueMesssage(queuedMessage, domain.Key);
                }
            }
        }

        private bool ProcessLocalRecipients(IQueuedMessage queuedMessage)
        {
            _logger.LogInformation("Processing local recipients for QueuedMessage: {0}", queuedMessage);

            queuedMessage.Envelopes.ForEach(async envelope =>
            {
                var user = _serverRepository.FindByEmailAddress(envelope.Recipient.FullAddress);

                if (user != null)
                {
                    try
                    {
                        _logger.LogDebug("Saving MessageId {0} for User: {1}", envelope.MessageId, user);
                        var path = Path.Combine(user.MailboxPath, envelope.MessageId + ".eml");
                        Directory.CreateDirectory(user.MailboxPath);

                        using (var fs = new FileStream(path, FileMode.Create))
                        {
                            await queuedMessage.MessageData.WriteToAsync(fs);
                        }

                        envelope.DeliveryAttempts++;
                        envelope.Disposition = Disposition.Delivered;
                        envelope.LastResponse = $"250 2.6.0 Message <{envelope.MessageId}> delivered locally for <{envelope.Recipient.FullAddress}>";
                    }
                    finally
                    {
                        _envelopeRepository.Update(envelope);
                        if (envelope.DeliveryComplete)
                        {
                            await _envelopeRepository.ArchiveAsync(envelope);
                        }
                    }
                }
            });

            if (queuedMessage.DeliveryComplete)
            {
                queuedMessage.MessageData.DeleteMessageData();
            }

            return queuedMessage.DeliveryComplete;
        }
    }
}