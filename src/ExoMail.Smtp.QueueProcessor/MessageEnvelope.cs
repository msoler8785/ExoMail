using ExoMail.Smtp.Common.Enums;
using ExoMail.Smtp.Common.Models;
using Newtonsoft.Json;
using System;

namespace ExoMail.Smtp.QueueProcessor
{
    public class MessageEnvelope : IMessageEnvelope
    {
        public int DeliveryAttempts { get; set; }
        public bool DeliveryComplete => Disposition.HasDisposition(Disposition.Delivered | Disposition.PermFail);
        public bool DeliveryRetryable => Disposition.HasDisposition(Disposition.Retryable);
        public Disposition Disposition { get; set; }

        [JsonIgnore]
        public string Domain => Recipient.Domain;

        public string EnvelopePath { get; set; }
        public string LastResponse { get; set; }
        public string MessageId { get; set; }
        public string MessagePath { get; set; }
        public MailAddress Recipient { get; set; }
        public string RemoteIp { get; set; }
        public MailAddress Sender { get; set; }
        public string SendingUserId { get; set; }
        public DateTime SentDate { get; set; }
        public DateTime QueueDate { get; set; }
        public DateTime NextAttempt { get; set; }

        public MessageEnvelope()
        {
        }

        public MessageEnvelope(MailAddress sender, MailAddress recipient)
        {
            DeliveryAttempts = 0;
            Disposition = Disposition.Queued;
            NextAttempt = DateTime.MinValue;
            QueueDate = DateTime.Now;
            Sender = sender;
            Recipient = recipient;
        }

        public override string ToString()
        {
            return $"MessageId: {MessageId} Recipient: {Recipient.FullAddress} Sender: {Sender.FullAddress}";
        }
    }
}