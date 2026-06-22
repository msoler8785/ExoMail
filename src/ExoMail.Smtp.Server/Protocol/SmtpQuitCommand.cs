using ExoMail.Smtp.Server.Utilities;
using System;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public sealed class SmtpQuitCommand : SmtpCommandBase
    {
        public SmtpQuitCommand()
            : base()
        {
        }

        public override bool ValidateArgs(out string argumentsResponse)
        {
            argumentsResponse = String.Empty;
            bool result = this.Arguments.Count == 0;

            if (!result)
                argumentsResponse = SmtpResponse.ArgumentUnrecognized;

            return result;
        }

        public override async Task ExecuteAsync()
        {
            string response;

            if (ValidateArgs(out response))
            {
                await this.SmtpSession.SendResponseAsync(String.Format(SmtpResponse.Closing, this.SmtpSession._serverConfig.HostName));
                this.SmtpSession.Reset();
                this.SmtpSession.TokenSource.Cancel();
            }
            else
            {
                await this.SmtpSession.SendResponseAsync(response);
            }
        }
    }
}