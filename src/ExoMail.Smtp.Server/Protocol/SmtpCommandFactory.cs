using ExoMail.Smtp.Common.Text;
using ExoMail.Smtp.Server.Enums;
using ExoMail.Smtp.Server.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ExoMail.Smtp.Server.Protocol
{
    public class SmtpCommandFactory
    {
        private SmtpSession _smtpSession;
        private TokenEnumerator _tokenEnumerator;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public SmtpCommandFactory(SmtpSession smtpSession, ILoggerFactory loggerFactory)
        {
            _smtpSession = smtpSession;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<SmtpCommandFactory>();
            _tokenEnumerator = new TokenEnumerator();
        }

        private SmtpCommandType ParseCommandType(string command)
        {
            SmtpCommandType commandType;

            if (Enum.TryParse<SmtpCommandType>(command, out commandType))
            {
                return commandType;
            }
            else
            {
                return SmtpCommandType.INVALID;
            }
        }

        public SmtpCommandBase Parse(string commandLine)
        {
            if (String.IsNullOrWhiteSpace(commandLine))
                return new SmtpUnknownCommand() { SmtpSession = _smtpSession };

            SmtpCommandBase smtpCommand;

            var arguments = _tokenEnumerator.GetTokens(commandLine);
            var commandArg = arguments.ElementAtOrDefault(0);
            var command = commandArg.Key.ToUpper();
            arguments.Remove(command);

            switch (command)
            {
                case "HELO":
                    smtpCommand = new SmtpHeloCommand();
                    break;

                case "EHLO":
                    smtpCommand = new SmtpEhloCommand();
                    break;

                case "STARTTLS":
                    smtpCommand = new SmtpStartTlsCommand();
                    break;

                case "MAIL":
                    smtpCommand = new SmtpMailCommand();
                    break;

                case "RCPT":
                    smtpCommand = new SmtpRcptCommand();
                    break;

                case "DATA":
                    smtpCommand = new SmtpDataCommand(_loggerFactory);
                    break;

                //case "BDAT":
                //    smtpCommand = new SmtpBdatCommand();
                //    break;

                case "RSET":
                    smtpCommand = new SmtpRsetCommand();
                    break;

                case "QUIT":
                    smtpCommand = new SmtpQuitCommand();
                    break;

                case "NOOP":
                    smtpCommand = new SmtpNoopCommand();
                    break;

                case "AUTH":
                    smtpCommand = new SmtpAuthCommand();
                    break;

                case "HELP":
                    smtpCommand = new SmtpHelpCommand();
                    break;

                case "VRFY":
                    smtpCommand = new SmtpVrfyCommand();
                    break;

                case "TURN":
                    smtpCommand = new SmtpTurnCommand();
                    break;

                default:
                    smtpCommand = new SmtpUnknownCommand();
                    break;
            }

            smtpCommand.SmtpSession = _smtpSession;
            smtpCommand.Command = command;
            smtpCommand.CommandLine = commandLine;
            smtpCommand.Arguments = arguments;

            if (smtpCommand.AllowedStates.ContainsFlag(_smtpSession.SmtpStateTable.SessionState))
            {
                return smtpCommand;
            }

            return new SmtpInvalidCommand(_smtpSession);
        }
    }
}