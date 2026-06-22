using ExoMail.Smtp.Common;
using ExoMail.Smtp.Common.Extensions;
using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Common.Models;
using ExoMail.Smtp.QueueProcessor;
using ExoMail.Smtp.Sdk;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Service
{
    public class Startup : IStartup
    {
        private static JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        private readonly ILogger<Startup> _logger;
        private readonly QueueOptions _options;
        private readonly IMessageEnvelopeRepository _repository;
        private readonly IDefaultRouter _defaultRouter;
        private readonly IVirtualServerRepository _userManager;

        public Startup(
            ILoggerFactory loggerFactory,
            IOptions<QueueOptions> options,
            IDefaultRouter defaultRouter,
            IMessageEnvelopeRepository repository,
            IVirtualServerRepository userManager)
        {
            _logger = loggerFactory.CreateLogger<Startup>();
            _options = options.Value;
            _repository = repository;
            _defaultRouter = defaultRouter ;
            _userManager = userManager;
        }

        public async Task Initialize()
        {
            //InitializeDbs();
            CreateDirectories();
            PurgeInvalidEnvelopes();
            await RebuildQueues();
        }

        //private void InitializeDbs()
        //{
        //    using (var db = new EmailDbContext())
        //    {
        //        db.Database.Migrate();

        //        var connection = db.Database.GetDbConnection();
        //        connection.Open();

        //        using (var command = connection.CreateCommand())
        //        {
        //            command.CommandText = "PRAGMA journal_mode=WAL;";
        //            command.ExecuteNonQuery();
        //        }

        //        if (!db.SmtpServers.Any())
        //        {
        //            db.SmtpServers.AddRange(SmtpServerConfig.GetDefaults());
        //        }
        //        db.SaveChanges();
        //    }
        //}

        private async Task RebuildQueues()
        {
            await Task.Run(() =>
            {
                _logger.LogInformation("Rebuilding queues.");
                var messages = Directory.EnumerateFiles(_options.QueuePath, "*.eml");

                foreach (var message in messages)
                {
                    QueuedMessage queuedMessage = new QueuedMessage(message, _repository, _userManager);

                    _logger.LogInformation("Requeuing message {0}", message);

                    _defaultRouter.RouteAsync(queuedMessage).RunAsync();
                }
            });
        }

        private void PurgeInvalidEnvelopes()
        {
            _logger.LogInformation("Purging invalid envelopes.");
            var envelopeFiles = Directory.EnumerateFiles(_options.QueuePath, "*.env.json");

            foreach (var file in envelopeFiles)
            {
                string json = File.ReadAllText(file);

                try
                {
                    IMessageEnvelope envelope = JsonConvert.DeserializeObject<IMessageEnvelope>(json, serializerSettings);

                    if (!File.Exists(envelope.MessagePath))
                    {
                        _logger.LogDebug("Purging invalid envelope {0}", file);
                        File.Delete(envelope.EnvelopePath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.HResult, ex, ex.Message);
                }
            }
        }

        private void CreateDirectories()
        {
            Directory.CreateDirectory(_options.QueuePath);
            Directory.CreateDirectory(_options.ArchivePath);
        }
    }
}