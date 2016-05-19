using ExoMail.Smtp.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    internal class SmtpHelpCommand : SmtpCommandBase
    {
        public SmtpHelpCommand(string command, List<string> arguments)
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
            return await Task.Run(() => GetHelp());
        }

        private string GetHelp()
        {
            var sb = new StringBuilder();

            string commandList = "EHLO HELO NOOP RSET QUIT MAIL RCPT RSET DATA AUTH HELP";

            if (this.SmtpSession.ServerConfig.IsStartTlsSupported)
            {
                commandList += " STARTTLS";
            }

            sb.AppendLine("214-This server supports the following commands:");
            sb.AppendFormat("214 {0}", commandList);

            return sb.ToString();
        }
    }
}