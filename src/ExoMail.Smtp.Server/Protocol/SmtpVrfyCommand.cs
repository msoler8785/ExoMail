using ExoMail.Smtp.Server.Utilities;
using System;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public sealed class SmtpVrfyCommand : SmtpCommandBase
    {
        public SmtpVrfyCommand()
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
            string response = SmtpResponse.CannotVrfy;

            await SmtpSession.SendResponseAsync(response);
        }
    }
}