using ExoMail.Smtp.Common.Enums;
using ExoMail.Smtp.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExoMail.Smtp.QueueProcessor
{
    public class NullMessageGateway : IMessageGateway
    {
        private readonly IMessageEnvelopeRepository _repository;

        [JsonIgnore]
        public IGatewayOptions GatewayOptions { get; set; }

        public NullMessageGateway(IMessageEnvelopeRepository repository)
        {
            _repository = repository;
        }

        public async Task EnqueueMessage(IQueuedMessage message, string domain)
        {
            var envelopes = message.Envelopes
                .Where(x => x.DeliveryRetryable)
                .Where(x => x.Domain.Equals(domain, StringComparison.OrdinalIgnoreCase))
                .ToList();
            var date = DateTime.Now;

            foreach (var envelope in envelopes)
            {
                envelope.DeliveryAttempts++;
                envelope.Disposition = Disposition.Delivered;
                envelope.SentDate = date;
                envelope.LastResponse = $"250 2.0.0 Message {envelope.MessageId} delivered to null route.";

                _repository.Update(envelope);
                await _repository.ArchiveAsync(envelope);
            }

            if (message.DeliveryComplete)
            {
                message.MessageData.DeleteMessageData();
            }
        }
    }
}