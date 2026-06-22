using System.Net;

namespace ExoMail.Smtp.Service
{
    public interface IDnsConfig
    {
        IPAddress[] Servers { get; }
    }
}