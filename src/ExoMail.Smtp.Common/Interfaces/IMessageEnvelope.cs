using ExoMail.Smtp.Common.Enums;
using ExoMail.Smtp.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Common.Models
{
    public interface IMessageEnvelope
    {
        int DeliveryAttempts { get; set; }
        bool DeliveryComplete { get; }
        bool DeliveryRetryable { get; }
        Disposition Disposition { get; set; }
        string Domain { get; }
        string EnvelopePath { get; set; }
        string LastResponse { get; set; }
        string MessageId { get; set; }
        string MessagePath { get; set; }
        string RemoteIp { get; }
        DateTime NextAttempt { get; set; }
        DateTime QueueDate { get; }
        DateTime SentDate { get; set; }
        MailAddress Recipient { get; }
        MailAddress Sender { get; }
        string SendingUserId { get; }
    }
}