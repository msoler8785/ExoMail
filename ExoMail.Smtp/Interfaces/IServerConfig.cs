using ExoMail.Smtp.Enums;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace ExoMail.Smtp.Interfaces
{
    public interface IServerConfig
    {
        string HostName { get; set; }
        bool IsEncryptionRequired { get; }
        bool IsTls { get; set; }
        int MaxMessageSize { get; set; }
        int Port { get; set; }
        string ServerId { get; set; }
        IPAddress ServerIpBinding { get; set; }
        int SessionTimeout { get; set; }
        //List<ISessionValidator> SessionValidators { get; set; }
        //List<IMessageValidator> MessageValidators { get; set; }
        X509Certificate2 X509Certificate2 { get; set; }
        ServerType ServerType { get; set; }

        //IMessageStore MessageStore { get; set; }
    }
}