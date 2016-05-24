using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    public class SmtpQuitCommand : SmtpCommandBase
    {
        public SmtpQuitCommand(string command, List<string> arguments)
        {
            Command = command;
            Arguments = arguments;
        }

        public override bool ArgumentsValid
        {
            get
            {
                return this.Arguments.Count == 0;
            }
        }

        public override async Task<string> GetResponseAsync()
        {
            return await Task.Run(() => GetResponse());
        }

        private string GetResponse()
        {
            string response;
            if (this.ArgumentsValid)
            {
                this.SmtpSession.Reset();
                this.SmtpSession.SmtpCommands.Add(this);
                response = String.Format(SmtpResponse.Closing, this.SmtpSession.ServerConfig.HostName);
            }
            else
            {
                response = SmtpResponse.ArgumentUnrecognized;
            }
            return response;
        }
    }
}