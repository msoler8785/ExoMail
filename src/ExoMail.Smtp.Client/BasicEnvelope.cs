using ExoMail.Smtp.Common.Enums;
using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Common.Models;
using System;

namespace ExoMail.Smtp.Client
{
    internal sealed class BasicEnvelope : IMessageEnvelope
    {
        public int DeliveryAttempts { get; set; }
        public bool DeliveryComplete { get; set; }
        public bool DeliveryRetryable { get; set; }
        public Disposition Disposition { get; set; }
        public string Domain { get; set; }
        public string EnvelopePath { get; set; }
        public string LastResponse { get; set; }
        public string MessageId { get; set; }
        public string MessagePath { get; set; }
        public DateTime NextAttempt { get; set; }
        public DateTime QueueDate { get; set; }
        public MailAddress Recipient { get; set; }
        public string RemoteIp { get; set; }
        public MailAddress Sender { get; set; }
        public string SendingUserId { get; set; }
        public DateTime SentDate { get; set; }
    }
}