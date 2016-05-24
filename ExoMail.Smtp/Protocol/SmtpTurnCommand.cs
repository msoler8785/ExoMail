using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    public class SmtpTurnCommand : SmtpCommandBase
    {
        public SmtpTurnCommand(string command, List<string> arguments)
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
            return await Task.Run(() => { return SmtpResponse.CommandNotImplemented; });
        }
    }
}