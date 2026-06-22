using ExoMail.Smtp.Client.Network;

namespace ExoMail.Smtp.Client.Interfaces
{
    internal interface IArgument
    {
        string Create(SmtpClient smtpClient);
    }
}