using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Exceptions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;
using System.Linq;

namespace ExoMail.Smtp.Models
{
    public class SmtpCommand
    {
        private string _command;

        public string Command
        {
            get { return _command.ToUpper(); }
            set { _command = value; }
        }

        private List<string> _argument;

        public List<string> Arguments
        {
            get { return _argument; }
            set { _argument = value; }
        }

        public SmtpCommandType CommandType
        {
            get { return ParseCommandType(); }
        }

        public SmtpCommand()
        {
            this.Command = String.Empty;
            this.Arguments = new List<string>();
        }

        private SmtpCommandType ParseCommandType()
        {
            SmtpCommandType commandType;

            if (Enum.TryParse<SmtpCommandType>(this.Command, out commandType)) return commandType;
            else return SmtpCommandType.INVALID;
        }

        public static SmtpCommand Parse(string commandLine)
        {
            List<string> commands = commandLine.Split(' ').ToList();

            string command = commands.ElementAtOrDefault(0) ?? String.Empty;
            var arguments = commands.Skip(1).ToList();
            //string command = commandLine.Substring(0, 4);
            //string argument = Regex.Match(commandLine, @"\s.*").Value;
            return new SmtpCommand() { Command = command, Arguments = arguments };
        }
    }
}