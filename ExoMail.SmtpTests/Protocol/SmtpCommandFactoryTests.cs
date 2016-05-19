using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExoMail.Smtp.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExoMail.Smtp.Enums;

namespace ExoMail.Smtp.Protocol.Tests
{
	[TestClass()]
	public class SmtpCommandFactoryTests
	{
		private SmtpCommandFactory factory { get; set; }
		private SmtpSession smtpSession { get; set; }

		string _heloCommand = "HELO mail.example.com";
		string _ehloCommand = "EHLO mail.example.com";
		string _mailCommand = "MAIL FROM:<fbar@example.com>";
		string _rcptCommand = "RCPT TO:<fbar@example.net>";
		string _helpCommand = "HELP";
		string _noopCommand = "NOOP";
		string _quitCommand = "QUIT";
		string _startTlsCommand = "STARTTLS";
		string _dataCommand = "DATA";
		string _rsetCommand = "RSET";
		string _turnCommand = "TURN";
		string _vrfyCommand = "VRFY";

		public SmtpCommandFactoryTests()
		{
			smtpSession = new SmtpSession();
			factory = new SmtpCommandFactory(smtpSession);
		}

		[TestMethod()]
		public void HeloCommandTest()
		{
			var command = factory.Parse(this._heloCommand);
			Assert.IsInstanceOfType(command, typeof(SmtpHeloCommand));
		}

		[TestMethod]
		public void EhloCommandTest()
		{
			var command = factory.Parse(this._ehloCommand);
			Assert.IsInstanceOfType(command, typeof(SmtpEhloCommand));
		}

		[TestMethod]
		public void MailCommandTest()
		{
			var command = factory.Parse(this._mailCommand);
			Assert.IsInstanceOfType(command, typeof(SmtpMailCommand));
		}

		[TestMethod]
		public void RcptCommandTest()
		{
			var command = factory.Parse(this._rcptCommand);
			Assert.IsInstanceOfType(command, typeof(SmtpRcptCommand));
		}

		[TestMethod]
		public void DataCommandTest()
		{
			var command = factory.Parse(this._dataCommand);
			Assert.IsInstanceOfType(command, typeof(SmtpDataCommand));
		}

		[TestMethod]
		public void RsetCommandTest()
		{
			var command = factory.Parse(this._rsetCommand);
			Assert.IsInstanceOfType(command, typeof(SmtpRsetCommand));
		}
	}
}