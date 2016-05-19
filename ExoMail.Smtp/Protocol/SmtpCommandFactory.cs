using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
	public class SmtpCommandFactory
	{
		private SmtpSession _smtpSession { get; set; }

		public SmtpCommandFactory(SmtpSession smtpSession)
		{
			this._smtpSession = smtpSession;
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
			SmtpCommandBase smtpCommand;

			List<string> commands = commandLine.Split(' ').ToList();

			string command = commands.ElementAtOrDefault(0) ?? String.Empty;
			var arguments = commands.Skip(1).ToList();

			switch (command.ToUpper())
			{
				case "HELO":
					smtpCommand =  new SmtpHeloCommand(command, arguments);
					break;
				case "EHLO":
					smtpCommand =  new SmtpEhloCommand(command, arguments);
					break;
				case "STARTTLS":
					smtpCommand = new SmtpStartTlsCommand(command, arguments);
					break;
				case "MAIL":
					smtpCommand =  new SmtpMailCommand(command, arguments);
					break;
				case "RCPT":
					smtpCommand =  new SmtpRcptCommand(command, arguments);
					break;
				case "DATA":
					smtpCommand =  new SmtpDataCommand(command, arguments);
					break;
				case "RSET":
					smtpCommand =  new SmtpRsetCommand(command, arguments);
					break;
				case "QUIT":
					smtpCommand =  new SmtpQuitCommand(command, arguments);
					break;
				case "NOOP":
					smtpCommand =  new SmtpNoopCommand(command, arguments);
					break;
				case "AUTH":
					smtpCommand =  new SmtpAuthCommand(command, arguments);
					break;
				case "HELP":
					smtpCommand =  new SmtpHelpCommand(command, arguments);
					break;
				case "SAML":
					smtpCommand =  new SmtpVrfyCommand(command, arguments);
					break;
				case "TURN":
					smtpCommand =  new SmtpTurnCommand(command, arguments);
					break;
				default:
                    smtpCommand = new SmtpUnknownCommand();
                    break;
			}

            smtpCommand.SmtpSession = _smtpSession;
			return smtpCommand;
		}
	}
}
