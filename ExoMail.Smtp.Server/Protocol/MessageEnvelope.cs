using ExoMail.Smtp.Server.Interfaces;
using ExoMail.Smtp.Server.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExoMail.Smtp.Server.Protocol
{
    [Serializable]
    public class MessageEnvelope : IMessageEnvelope
    {
        public string MessageId { get; set; }
        public string SenderDomain { get; set; }
        public string SenderAddress { get; set; }
        public List<Recipient> Recipients { get; set; }

        public List<string> RecipientDomains
        {
            get
            {
                return this.Recipients
                    .Select(r => r.RecipientDomain)
                    .Distinct()
                    .ToList();
            }
        }

        public string MessagePath { get; set; }

        public MessageEnvelope()
        {
            this.Recipients = new List<Recipient>();
            this.MessageId = Guid.NewGuid().ToString();
        }

        public void SetSenderAddress(string senderAddress)
        {
            this.SenderAddress = senderAddress;
        }

        public void SetSenderDomain(string domain)
        {
            this.SenderDomain = domain;
        }

        public void AddRecipient(string recipientAddress, string recipientDomain)
        {
            this.Recipients.Add(new Recipient()
            {
                RecipientAddress = recipientAddress,
                RecipientDomain = recipientDomain
            });
        }

        public void SaveEnvelope(string path)
        {
            this.MessagePath = path;
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);

            File.WriteAllText(path + ".env", json);
            Reset();
        }

        public void Reset()
        {
            this.Recipients.Clear();
            this.SenderAddress = null;
            this.MessagePath = null;
            this.MessageId = Guid.NewGuid().ToString();
        }

        public static IMessageEnvelope Load(string fullPath)
        {
            var json = File.ReadAllText(fullPath);

            MessageEnvelope envelope = JsonConvert.DeserializeObject<MessageEnvelope>(json);

            return envelope;
        }
    }
}