using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net;
using MimeKit;
using Newtonsoft.Json;
using System.IO;

namespace ExoMail.Smtp.Models
{
    public class DeliveryAgent
    {
        public string MessagePath { get; set; }

        public List<string> Recipients { get; set; }

        [JsonIgnore]
        public string Message { get; set; }
        public string MessageId { get; set; }

        [JsonIgnore]
        private string _deliveryAgentFile { get; set; }

        private DeliveryAgent()
        {
            this.Recipients = new List<string>();
        }

        public static DeliveryAgent Create(string path, List<string> recipients, string messageId)
        {
            var agent = new DeliveryAgent() { MessagePath = path, Recipients = recipients, MessageId = messageId };
            agent.Save();
            return agent;
        }

        private void Save()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            this._deliveryAgentFile = json;
            File.WriteAllText(this.MessagePath + ".que", json);
        }

        public static DeliveryAgent Load(string deliveryAgent)
        {
            using (var stream = new FileStream(deliveryAgent, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                DeliveryAgent agent = JsonConvert.DeserializeObject<DeliveryAgent>(json);
                return agent;
            }

        }
    }
}
