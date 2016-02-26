using System;
using System.Collections.Generic;
namespace ExoMail.Smtp.Interfaces
{
    interface ISessionConfig
    {
        string HostName { get; set; }
        int MaxBadCommands { get; set; }
        int MaxMessageSize { get; set; }
        IMessageStore MessageStore { get; set; }
        int SessionTimeout { get; set; }
        List<ISessionValidator> SessionValidators { get; set; }
    }
}
