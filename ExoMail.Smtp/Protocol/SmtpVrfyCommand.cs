using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    public class SmtpVrfyCommand : SmtpCommandBase
    {
        public SmtpVrfyCommand(string command, List<string> arguments)
        {
            Command = command;
            Arguments = arguments;
        }

        public override bool ArgumentsValid
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override async Task<string> GetResponseAsync()
        {
            return await Task.Run(() => { return SmtpResponse.CannotVrfy; });
        }
    }
}