using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    internal class SmtpNoopCommand : SmtpCommandBase
    {
        public SmtpNoopCommand(string command, List<string> arguments)
        {
            Command = command;
            Arguments = arguments;
        }

        public override bool ArgumentsValid
        {
            get
            {
                return this.Arguments.Count <= 1;
            }
        }
        public override async Task<string> GetResponseAsync()
        {
            return await Task.Run(() => GetResponse());
        }

        private string GetResponse()
        {
            if (this.ArgumentsValid)
            {
                return SmtpResponse.OK;
            }
            else
            {
                return SmtpResponse.ArgumentUnrecognized;
            }
        }
    }
}