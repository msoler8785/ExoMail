using ExoMail.Smtp.Common;
using ExoMail.Smtp.Common.Extensions;
using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Common.Models;
using ExoMail.Smtp.QueueProcessor;
using ExoMail.Smtp.Sdk;
using ExoMail.Smtp.Server.Interfaces;
using ExoMail.Smtp.Server.Protocol;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Service.Services
{
    public class FileMessageStore : IMessageStore
    {
        private readonly IMessageEnvelopeRepository _envelopeRepository;
        private readonly QueueOptions _options;
        private readonly PluginOptions _pluginOptions;
        private readonly IEnumerable<IMessageProcessor> _messsageProcessors;
        private readonly IDefaultRouter _defaultRouter;
        private readonly IVirtualServerRepository _userManager;
        private readonly ILogger<FileMessageStore> _logger;

        public FileMessageStore(
            IMessageEnvelopeRepository envelopeRepository,
            IOptions<QueueOptions> options,
            IOptions<PluginOptions> pluginOptions,
            IEnumerable<IMessageProcessor> messsageProcessors,
            IDefaultRouter defaultRouter,
            IVirtualServerRepository userManager,
            ILoggerFactory loggerFactory
            )
        {
            _envelopeRepository = envelopeRepository;
            _options = options.Value;
            _pluginOptions = pluginOptions.Value;
            _messsageProcessors = messsageProcessors;
            _defaultRouter = defaultRouter;
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<FileMessageStore>();
        }

        public async Task<string> Save(SmtpSession smtpSession, Stream message)
        {
            _logger.LogDebug("Saving message from SmtpSession with SessionId: {0}", smtpSession.SessionId);

            CreateQueuedMessage(
                smtpSession,
                out string messageId,
                out string messagePath,
                out IMessageData messageData,
                out QueuedMessage queuedMessage
                );

            CreateEnvelopes(smtpSession, messageId, messagePath, queuedMessage);
            await StoreMessageData(smtpSession, message, messageId, messageData);
            ProcessMessageAsync(queuedMessage).RunAsync();

            return messageId;
        }

        private async Task StoreMessageData(SmtpSession smtpSession, Stream message, string messageId, IMessageData messageData)
        {
            await messageData.ReadFromAsync(message);
            string received = new SmtpReceivedHeader(smtpSession, messageId).GetReceivedHeaders();

            if (String.IsNullOrEmpty(messageData.MimeMesage.MessageId))
                messageData.MimeMesage.MessageId = messageId;

            messageData.MimeMesage.Headers.Insert(0, HeaderId.Received, received);

            await messageData.SaveChangesAsync();
        }

        private void CreateEnvelopes(SmtpSession smtpSession, string messageId, string messagePath, QueuedMessage queuedMessage)
        {
            foreach (var recipient in smtpSession.SmtpStateTable.Recipients)
            {
                string envelopePath = Path.Combine(_options.QueuePath, $"{messageId}.{recipient.FullAddress}.env.json");

                _logger.LogDebug("Creating MessageEnvelope. SessionId: {0} MessageId:{1}", smtpSession.SessionId, messageId);
                IMessageEnvelope envelope = new MessageEnvelope(smtpSession.SmtpStateTable.Sender, recipient)
                {
                    EnvelopePath = envelopePath,
                    MessagePath = messagePath,
                    MessageId = messageId,
                    SendingUserId = smtpSession.SmtpStateTable.User.UserId
                };

                _envelopeRepository.Create(envelope);
                queuedMessage.Envelopes.Add(envelope);
            }
        }

        private void CreateQueuedMessage(SmtpSession smtpSession, out string messageId, out string messagePath, out IMessageData messageData, out QueuedMessage queuedMessage)
        {
            messageId = $"{Guid.NewGuid()}@{Environment.MachineName}";
            messagePath = Path.Combine(_options.QueuePath, messageId + ".eml");
            _logger.LogDebug("Creating MessageData. SessionId: {0} MessageId:{1}", smtpSession.SessionId, messageId);
            messageData = new MessageData(messagePath);
            IVirtualServer virtualServer = _userManager.FindUserVirtualServer(smtpSession.SmtpStateTable.User.UserId);

            _logger.LogDebug("Creating QueuedMessage. SessionId: {0} MessageId:{1} VirtualServer: {2}", smtpSession.SessionId, messageId, virtualServer.VirtualServerId);
            queuedMessage = new QueuedMessage(messageData, virtualServer)
            {
                Sender = smtpSession.SmtpStateTable.Sender
            };
        }

        private async Task ProcessMessageAsync(IQueuedMessage queuedMessage)
        {
            var processors = _messsageProcessors
                .Where(plugin => _pluginOptions.IsEnabled(plugin))
                .OrderByDescending(plugin => plugin.Priority);

            foreach (var processor in processors)
            {
                await processor.ProcessAsync(queuedMessage).ConfigureAwait(false);

                if (processor.StopProcessing) break;
            }

            if (_options.DefaultRouteEnabled)
            {
                await _defaultRouter.RouteAsync(queuedMessage);
            }
        }
    }
}