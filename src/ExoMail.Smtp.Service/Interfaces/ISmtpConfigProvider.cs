using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Server.Interfaces;
using System.Collections.Generic;

namespace ExoMail.Smtp.Service.Interfaces
{
    public interface ISmtpConfigProvider
    {
        IEnumerable<ISmtpServerConfig> Load();
    }
}