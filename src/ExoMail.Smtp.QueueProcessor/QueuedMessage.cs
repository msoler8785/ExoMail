using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExoMail.Smtp.QueueProcessor
{
    public class QueuedMessage : IQueuedMessage
    {
        private readonly IMessageData _messageData;
        private readonly IVirtualServerRepository _userManager;
        private readonly IVirtualServer _virtualServer;

        public TimeSpan Delay
        {
            get
            {
                TimeSpan delay = Envelopes.Min(x => x.NextAttempt).Subtract(DateTime.Now);

                if (delay < TimeSpan.Zero)
                    delay = TimeSpan.Zero;

                return delay;
            }
        }

        public bool DeliveryComplete => Envelopes.All(x => x.DeliveryComplete);

        public IDkimPolicy DkimPolicy
        {
            get
            {
                IDkimPolicy dkimPolicy = null;
                var mailboxAddress = _messageData.MimeMesage.From?.Mailboxes.FirstOrDefault()?.Address;

                if (MailAddress.TryParse(mailboxAddress, false, out MailAddress mailAddress))
                {
                    var policies = _virtualServer?.DkimPolicies;
                    dkimPolicy = policies?.FirstOrDefault(x =>
                        x.Domain.Equals(mailAddress.Domain, StringComparison.OrdinalIgnoreCase));
                }

                return dkimPolicy;
            }
        }

        public List<IMessageEnvelope> Envelopes { get; private set; }

        public IMessageData MessageData => _messageData;

        public MailAddress Sender { get; set; }

        public IVirtualServer VirtualServer => _virtualServer;

        public string SenderIpAddress => Envelopes.FirstOrDefault()?.RemoteIp;
        public string HeloDomain { get; set; }
        public string MessageId => Envelopes.FirstOrDefault()?.MessageId;

        public QueuedMessage(string messagePath, IMessageEnvelopeRepository repository, IVirtualServerRepository userManager)
        {
            _messageData = new MessageData(messagePath);
            _userManager = userManager;
            string messageId = Path.GetFileNameWithoutExtension(Path.GetFileName(messagePath));
            Envelopes = repository.FindAll(messageId).ToList();
            Sender = Envelopes.FirstOrDefault()?.Sender;
            _virtualServer = _userManager.FindUserVirtualServer(Envelopes.FirstOrDefault()?.SendingUserId);
        }

        public QueuedMessage(IMessageData messageData, IVirtualServer virtualServer)
        {
            Envelopes = new List<IMessageEnvelope>();
            _messageData = messageData;
            _virtualServer = virtualServer;
        }

        public override string ToString()
        {
            return $"Sender: {Sender.FullAddress} VirtualServerId: {_virtualServer.VirtualServerId} MessageId: {_messageData?.MimeMesage?.MessageId}";
        }
    }
}