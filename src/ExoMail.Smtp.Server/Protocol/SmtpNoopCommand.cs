using ExoMail.Smtp.Server.Utilities;
using System;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public sealed class SmtpNoopCommand : SmtpCommandBase
    {
        public SmtpNoopCommand()
            : base()
        {
        }

        public override bool ValidateArgs(out string argumentsResponse)
        {
            argumentsResponse = String.Empty;
            bool result = this.Arguments.Count <= 1;

            if (!result)
                argumentsResponse = SmtpResponse.ArgumentUnrecognized;

            return result;
        }

        public override async Task ExecuteAsync()
        {
            string response;
            if (ValidateArgs(out response))
            {
                response = SmtpResponse.OK;
            }
            await SmtpSession.SendResponseAsync(response);
        }
    }
}