using ExoMail.Smtp.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    public class MessageEnvelope : IMessageEnvelope
    {
        public string SenderDomain { get; set; }
        public string SenderEmail { get; set; }
        public List<string> Recipients { get; set; }
        public string MessagePath { get; set; }

        public MessageEnvelope()
        {
            this.Recipients = new List<string>();
        }

        public void SetSenderEmail(string senderEmail)
        {
            this.SenderEmail = senderEmail;
        }

        public void SetSenderDomain(string domain)
        {
            this.SenderDomain = domain;
        }

        public void AddRecipient(string recipient)
        {
            this.Recipients.Add(recipient);
        }

        public void SaveEnvelope(string path)
        {
            this.MessagePath = path  ;
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);

            File.WriteAllText(path + ".env", json);
            Reset();
        }

        public void Reset()
        {
            this.Recipients.Clear();
            this.SenderEmail = null;
            this.MessagePath = null;
        }
    }
}
