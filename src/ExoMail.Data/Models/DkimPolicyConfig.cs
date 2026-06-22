using ExoMail.Smtp.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using MimeKit.Cryptography;

namespace ExoMail.Data.Models
{
    public class DkimPolicyConfig
    {
        public int DkimPolicyConfigId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public bool UseDkim { get; set; }
        public string Domain { get; set; }
        public string Selector { get; set; }
        public string Headers { get; set; }

        public virtual Guid VirtualServerConfigId { get; set; }
        public virtual VirtualServerConfig VirtualServerConfig { get; set; }
    }
}
