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
            return await Task.Run(() => GetResponse());
        }

        private string GetResponse()
        {
            string response;
            if (ValidateArgs(out response))
            {
                this.SmtpSession.Reset();
                this.SmtpSession.SmtpCommands.Add(this);
                response = String.Format(SmtpResponse.Closing, this.SmtpSession.ServerConfig.HostName);
            }
         
            return response;
        }
    }
}