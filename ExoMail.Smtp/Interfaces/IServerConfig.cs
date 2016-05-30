using ExoMail.Smtp.Enums;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace ExoMail.Smtp.Interfaces
{
    public interface IServerConfig
    {
        string HostName { get; }
        bool IsAuthRequired { get; }
        bool IsEncryptionRequired { get; }
        bool IsStartTlsSupported { get; }
        bool IsAuthRelayAllowed { get; }
        bool IsTls { get; }
        int MaxMessageSize { get; }
        int Port { get; }
        string ServerId { get; }
        IPAddress ServerIpBinding { get;  }
        int SessionTimeout { get; }
        X509Certificate2 X509Certificate2 { get; }
    }
}