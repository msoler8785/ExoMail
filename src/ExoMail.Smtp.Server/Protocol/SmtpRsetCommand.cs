using ExoMail.Smtp.Server.Utilities;
using System;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public sealed class SmtpRsetCommand : SmtpCommandBase
    {
        public SmtpRsetCommand()
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
                response = SmtpResponse.Resetting;
                this.SmtpSession.Reset();
            }
            await SmtpSession.SendResponseAsync(response);
        }
    }
}