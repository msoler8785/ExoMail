using ExoMail.Smtp.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    public abstract class SmtpCommandBase
    {
        private List<string> _argument;
        private string _command;

        public List<string> Arguments
        {
            get { return _argument; }
            set { _argument = value; }
        }

        public abstract bool ArgumentsValid { get; }

        public string Command
        {
            get { return _command.ToUpper(); }
            set { _command = value; }
        }

        public SmtpCommandType CommandType
        {
            get { return ParseCommandType(); }
        }

        public bool IsValid { get; set; }
        public SmtpSession SmtpSession { get; set; }

        public SmtpCommandBase()
        {
            this.Command = String.Empty;
            this.Arguments = new List<string>();
            this.IsValid = false;
        }

        public abstract Task<string> GetResponseAsync();

        public virtual async Task ProcessCommandAction()
        {
            await Task.Run(() =>
            {
                if (this.IsValid)
                {
                    this.SmtpSession.SmtpCommands.Add(this);
                }
            });
        }

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

        //public static SmtpCommandBase Parse(string commandLine)
        //{
        //    List<string> commands = commandLine.Split(' ').ToList();

        //    string command = commands.ElementAtOrDefault(0) ?? String.Empty;
        //    var arguments = commands.Skip(1).ToList();
        //    //string command = commandLine.Substring(0, 4);
        //    //string argument = Regex.Match(commandLine, @"\s.*").Value;
        //    return new SmtpCommandBase() { Command = command, Arguments = arguments };
        //}
    }
}