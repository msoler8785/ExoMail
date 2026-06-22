using ExoMail.Smtp.Common.Collections;
using ExoMail.Smtp.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoMail.Smtp.QueueProcessor
{
    public class MxMessageGateway : IMessageGateway
    {
        private readonly ISmtpClientResolver _smtpClientResolver;
        private readonly IMessageEnvelopeRepository _envelopeRepository;
        private volatile ExpirableDictionary<string, SmtpClientCache> _domainCaches;
        private readonly ILogger<MxMessageGateway> _logger;

        public IGatewayOptions GatewayOptions { get; set; }

        public MxMessageGateway(
            ISmtpClientResolver smtpClientResolver,
            IMessageEnvelopeRepository envelopeRepository,
            ILoggerFactory loggerFactory
            )
        {
            _smtpClientResolver = smtpClientResolver;
            _envelopeRepository = envelopeRepository;
            _domainCaches = new ExpirableDictionary<string, SmtpClientCache>(StringComparer.OrdinalIgnoreCase, loggerFactory);
            _logger = loggerFactory?.CreateLogger<MxMessageGateway>();
        }

        public async Task EnqueueMessage(IQueuedMessage queuedMessage, string domain)
        {
            SmtpClientCache smtpClientCache;

            lock (_domainCaches)
            {
                if (!_domainCaches.TryGetValue(domain, out smtpClientCache))
                {
                    smtpClientCache = new SmtpClientCache(domain, _smtpClientResolver, _envelopeRepository, GatewayOptions);

                    _domainCaches.Add(domain, smtpClientCache);
                }
            }

            await smtpClientCache.EnqueueMessage(queuedMessage);
        }
    }
}