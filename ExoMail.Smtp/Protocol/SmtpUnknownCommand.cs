using ExoMail.Smtp.Utilities;
using System;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    internal class SmtpUnknownCommand : SmtpCommandBase
    {

        public override bool ValidateArgs(out string argumentsResponse)
        {
            argumentsResponse = String.Empty;
            bool result = this.Arguments.Count == 0;

            if (!result)
                argumentsResponse = SmtpResponse.ArgumentUnrecognized;

            return result;
        }

        public override async Task<string> GetResponseAsync()
        {
            return await Task.Run(() => { return SmtpResponse.CommandUnrecognized; } );
        }
    }
}