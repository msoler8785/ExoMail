using ExoMail.Smtp.Server.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public abstract class SmtpCommandBase
    {
        public virtual SessionState AllowedStates
        {
            get
            {
                return
                    SessionState.AuthNeeded |
                    SessionState.DataNeeded |
                    SessionState.EhloNeeded |
                    SessionState.MailNeeded |
                    SessionState.RcptNeeded |
                    SessionState.StartTlsNeeded;
            }
        }

        public Dictionary<string, string> Arguments { get; set; }

        //public List<string> Arguments { get; set; }
        private string _command;

        public string Command
        {
            get { return _command; }
            set { _command = value; }
        }

        public SmtpCommandType CommandType
        {
            get { return ParseCommandType(); }
        }

        public string CommandLine { get; set; }

        public bool IsValid { get; set; }
        public SmtpSession SmtpSession { get; set; }

        public SmtpCommandBase()
        {
            this.Command = String.Empty;
            this.IsValid = false;
        }

        public abstract Task ExecuteAsync();

        public abstract bool ValidateArgs(out string argumentsResponse);

        private SmtpCommandType ParseCommandType()
        {
            SmtpCommandType commandType;

            if (Enum.TryParse<SmtpCommandType>(this.Command, out commandType))
            {
                return commandType;
            }
            else
            {
                return SmtpCommandType.INVALID;
            }
        }
    }
}