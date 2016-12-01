using ExoMail.Smtp.Server.Models;
using System.Collections.Generic;

namespace ExoMail.Smtp.Server.Interfaces
{
    public interface IMessageEnvelope
    {
        string MessageId { get; }
        string MessagePath { get; }
        List<Recipient> Recipients { get; }
        List<string> RecipientDomains { get; }
        string SenderDomain { get; }
        string SenderAddress { get; }
        void AddRecipient(string recipientAddress, string recipientDomain);
        void Reset();
        void SaveEnvelope(string path);
        void SetSenderDomain(string domain);
        void SetSenderAddress(string senderAddress);
    }
}