using ExoMail.Smtp.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace ExoMail.Smtp.Service.Configuration
{
    public class SmtpServerConfig : ISmtpServerConfig
    {
        public string ServerId { get; set; }
        public string HostName { get; set; }
        public bool IsAuthRequired { get; set; }
        public bool IsEncryptionRequired { get; set; }
        public bool IsStartTlsSupported { get; set; }
        public bool IsAuthRelayAllowed { get; set; }
        public bool IsTls { get; set; }
        public int MaxMessageSize { get; set; }
        public int Port { get; set; }
        public string ServerIpBindingString { get; set; } = IPAddress.Any.ToString();
        public int SessionTimeout { get; set; }
        public string CertificatePath { get; set; }
        public string InboundQueuePath { get; set; }

        [JsonIgnore]
        public IPAddress ServerIpBinding => IPAddress.Parse(ServerIpBindingString);

        private X509Certificate2 _x509Certificate2;
        [JsonIgnore]
        public X509Certificate2 X509Certificate2
        {
            get
            {
                if (_x509Certificate2 == null && !String.IsNullOrEmpty(CertificatePath))
                {
                    _x509Certificate2 = new X509Certificate2(CertificatePath);
                }
                return _x509Certificate2;
            }
        }

        public static IEnumerable<SmtpServerConfig> GetDefaults()
        {
            var servers = new List<SmtpServerConfig>()
            {
                new SmtpServerConfig()
                {
                    IsAuthRelayAllowed = true,
                    IsAuthRequired = false,
                    CertificatePath = "localhost.pfx",
                    HostName = Environment.MachineName,
                    InboundQueuePath = "Queues\\Inbound",
                    IsEncryptionRequired = false,
                    IsStartTlsSupported = true,
                    MaxMessageSize = 50 * 1024 * 1024,
                    Port = 25,
                    IsTls = false,
                    ServerIpBindingString = "0.0.0.0",
                    SessionTimeout = 5 * 60 * 1000,
                },
                new SmtpServerConfig()
                {
                    IsAuthRelayAllowed = true,
                    IsAuthRequired = true,
                    CertificatePath = "localhost.pfx",
                    HostName = Environment.MachineName,
                    InboundQueuePath = "Queues\\Inbound",
                    IsEncryptionRequired = true,
                    IsStartTlsSupported = true,
                    MaxMessageSize = 50 * 1024 * 1024,
                    Port = 587,
                    IsTls = false,
                    ServerIpBindingString = "0.0.0.0",
                    SessionTimeout = 5 * 60 * 1000,
                },
                new SmtpServerConfig()
                {
                    IsAuthRelayAllowed = true,
                    IsAuthRequired = true,
                    CertificatePath = "localhost.pfx",
                    HostName = Environment.MachineName,
                    InboundQueuePath = "Queues\\Inbound",
                    IsEncryptionRequired = true,
                    IsStartTlsSupported = false,
                    MaxMessageSize = 50 * 1024 * 1024,
                    Port = 465,
                    IsTls = true,
                    ServerIpBindingString = "0.0.0.0",
                    SessionTimeout = 5 * 60 * 1000,
                }
            };

            return servers;
        }
    }
}