using ExoMail.Smtp.Common;
using ExoMail.Smtp.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ExoMail.Smtp.QueueProcessor
{
    public class MessageEnvelopeRepository : IMessageEnvelopeRepository
    {
        private static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented
        };

        private readonly ILogger<MessageEnvelopeRepository> _logger;
        private readonly QueueOptions _queueOptions;

        public MessageEnvelopeRepository(ILoggerFactory loggerFactory, IOptions<QueueOptions> options)
        {
            _logger = loggerFactory.CreateLogger<MessageEnvelopeRepository>();
            _queueOptions = options.Value;
        }

        public async Task ArchiveAsync(IMessageEnvelope envelope)
        {
            _logger.LogDebug("Archiving MessageEnvelope: {0}", envelope.ToString());
            var fileName = Path.GetFileName(envelope.EnvelopePath);
            var destination = Path.Combine(_queueOptions.ArchivePath, fileName);

            await Task.Run(() => File.Move(envelope.EnvelopePath, destination));
        }

        public IMessageEnvelope Create(IMessageEnvelope item)
        {
            _logger.LogDebug("Creating MessageEnvelope: {0}", item.ToString());
            var json = JsonConvert.SerializeObject(item, item.GetType(), _jsonSettings);

            File.WriteAllText(item.EnvelopePath, json);

            return item;
        }

        public void Delete(IMessageEnvelope item)
        {
            _logger.LogDebug("Deleting MessageEnvelope: {0}", item.ToString());

            if (File.Exists(item.EnvelopePath))
                File.Delete(item.EnvelopePath);
        }

        public IMessageEnvelope Find(string id)
        {
            _logger.LogDebug("Finding MessageEnvelope with id: {0}", id);

            string json = File.ReadAllText(id);
            IMessageEnvelope envelope = JsonConvert.DeserializeObject<IMessageEnvelope>(json, _jsonSettings);

            return envelope;
        }

        public IEnumerable<IMessageEnvelope> FindAll(string messageId)
        {
            _logger.LogDebug("Finding all MessageEnvelopes with id: {0}", messageId);

            var files = Directory.EnumerateFiles(_queueOptions.QueuePath, $"{messageId}*.env.json");

            foreach (var file in files)
            {
                IMessageEnvelope envelope = Find(file);

                if (envelope.DeliveryRetryable)
                    yield return envelope;
            }
        }

        public void SaveChanges()
        {
            throw new System.NotImplementedException();
        }

        public void Update(IMessageEnvelope item)
        {
            _logger.LogDebug("Updating MessageEnvelope: {0}", item.ToString());

            string json = JsonConvert.SerializeObject(item, _jsonSettings);
            File.WriteAllText(item.EnvelopePath, json);
        }
    }
}