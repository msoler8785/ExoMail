using ExoMail.Smtp.Server.Utilities;
using System;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    internal class SmtpUnknownCommand : SmtpCommandBase
    {
        public override bool ValidateArgs(out string argumentsResponse)
        {
            argumentsResponse = String.Empty;

            return true;
        }

        public override async Task ExecuteAsync()
        {
            await SmtpSession.SendResponseAsync(SmtpResponse.CommandUnrecognized);
        }
    }
}