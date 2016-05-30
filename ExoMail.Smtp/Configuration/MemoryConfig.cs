using ExoMail.Smtp.Interfaces;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace ExoMail.Smtp.Configuration
{
    public class MemoryConfig : IServerConfig
    {
        public string HostName { get; set; }

        public bool IsAuthRequired { get; set; }

        public bool IsEncryptionRequired { get; set; }

        public bool IsStartTlsSupported { get; set; }

        public bool IsTls { get; set; }

        public int MaxMessageSize { get; set; }

        public int Port { get; set; }

        public string ServerId { get; set; }

        public IPAddress ServerIpBinding { get; set; }

        public int SessionTimeout { get; set; }

        public X509Certificate2 X509Certificate2 { get; set; }

        public bool IsAuthRelayAllowed { get; set; }

        public MemoryConfig()
        {
            HostName = Environment.MachineName;
            IsAuthRequired = false;
            IsEncryptionRequired = false;
            IsStartTlsSupported = false;
            IsTls = false;
            MaxMessageSize = int.MaxValue;
            Port = 25;
            ServerId = Guid.NewGuid().ToString();
            ServerIpBinding = IPAddress.Any;
            SessionTimeout = int.MaxValue;
            IsAuthRelayAllowed = false;
        }

        public static MemoryConfig Create()
        {
            return new MemoryConfig();
        }

        public MemoryConfig WithHostname(string hostName)
        {
            this.HostName = hostName;
            return this;
        }

        public MemoryConfig WithAuthenticationRequired()
        {
            this.IsAuthRequired = true;
            return this;
        }

        public MemoryConfig WithAuthRelayAllowed()
        {
            this.IsAuthRelayAllowed = true;
            return this;
        }

        public MemoryConfig WithStartTlsSupported()
        {
            this.IsStartTlsSupported = true;
            return this;
        }

        public MemoryConfig WithEncryptionRequired()
        {
            this.IsEncryptionRequired = true;
            return this;
        }

        public MemoryConfig WithTls()
        {
            IsEncryptionRequired = true;
            return this;
        }

        public MemoryConfig WithMaxMessageSize(int maxMessageSize)
        {
            MaxMessageSize = maxMessageSize;
            return this;
        }

        public MemoryConfig WithPort(int port)
        {
            Port = port;
            return this;
        }

        public MemoryConfig WithServerId(string serverId)
        {
            ServerId = serverId;
            return this;
        }

        public MemoryConfig WithServerIpBinding(IPAddress ipBinding)
        {
            ServerIpBinding = ipBinding;
            return this;
        }

        public MemoryConfig WithSessionTimeout(int timeout)
        {
            SessionTimeout = timeout;
            return this;
        }

        public MemoryConfig WithX509Certificate(string path, string password)
        {
            X509Certificate2 = new X509Certificate2(path, password);
            return this;
        }
    }
}