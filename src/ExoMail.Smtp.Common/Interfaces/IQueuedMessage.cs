using ExoMail.Smtp.Common.Models;
using System;
using System.Collections.Generic;

namespace ExoMail.Smtp.Common.Interfaces
{
    public interface IQueuedMessage
    {
        TimeSpan Delay { get; }
        bool DeliveryComplete { get; }
        List<IMessageEnvelope> Envelopes { get; }
        IMessageData MessageData { get; }
        MailAddress Sender { get; set; }
        IVirtualServer VirtualServer { get; }
        IDkimPolicy DkimPolicy { get; }
        string SenderIpAddress { get; }
        string HeloDomain { get; }
        string MessageId { get; }
    }
}