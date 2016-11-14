using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    public sealed class SmtpVrfyCommand : SmtpCommandBase
    {
        public SmtpVrfyCommand(string command, List<string> arguments)
        {
            Command = command;
            Arguments = arguments;
        }

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
            return await Task.Run(() => { return SmtpResponse.CannotVrfy; });
        }
    }
}