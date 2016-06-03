using System.Collections.Generic;

namespace ExoMail.Smtp.Interfaces
{
    public interface IMessageEnvelope
    {
        string MessageId { get; }
        string MessagePath { get; }
        List<string> Recipients { get; }
        string SenderDomain { get; }
        string SenderEmail { get; }

        void AddRecipient(string recipient);
        void Reset();
        void SaveEnvelope(string path);
        void SetSenderDomain(string domain);
        void SetSenderEmail(string senderEmail);
    }
}