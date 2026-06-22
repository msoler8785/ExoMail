using System;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public sealed class SmtpHelpCommand : SmtpCommandBase
    {
        public SmtpHelpCommand()
            : base()
        {
        }

        public override bool ValidateArgs(out string argumentsResponse)
        {
            argumentsResponse = String.Empty;

            return true;
        }

        public override async Task ExecuteAsync()
        {
            var sb = new StringBuilder();

            string commandList = "EHLO HELO NOOP RSET QUIT MAIL RCPT RSET DATA AUTH HELP";

            if (this.SmtpSession._serverConfig.IsStartTlsSupported)
            {
                commandList += " STARTTLS";
            }

            sb.AppendLine("214-This server supports the following commands:");
            sb.AppendFormat("214 {0}", commandList);

            await SmtpSession.SendResponseAsync(sb.ToString());
        }
    }
}