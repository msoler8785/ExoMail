using ExoMail.Smtp.Client.Enums;
using ExoMail.Smtp.Client.Extensions;
using ExoMail.Smtp.Client.Interfaces;
using ExoMail.Smtp.Client.Network;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Client.Protocol
{
    internal class SmtpMailCommand : SmtpCommandBase, ISmtpCommand
    {
        private long _size;
        private string _sender;

        public SmtpMailCommand(SmtpClient smtpClient, string sender, long size)
            : base(smtpClient)
        {
            _size = size;
            _sender = sender;
        }

        public override string CommandVerb => "MAIL";

        public async Task Invoke()
        {
            string commandLine = $"MAIL FROM:<{_sender}>";

            if (SmtpClient.ServerExtensions.HasExtension(ServerExtensions.SIZE))
                commandLine += $" SIZE={_size}";

            await SmtpClient.SendCommandAsync(commandLine);
        }
    }
}