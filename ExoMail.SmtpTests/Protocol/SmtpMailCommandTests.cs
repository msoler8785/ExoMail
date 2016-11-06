using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExoMail.Smtp.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Utilities;
using System.Net.Sockets;
using ExoMail.Smtp.Configuration;
namespace ExoMail.Smtp.Protocol.Tests
{
    [TestClass()]
    public class SmtpMailCommandTests : CommandTestBase<SmtpMailCommand>
    {
        public override SessionState TestSessionState => SessionState.MailNeeded;

        public SmtpMailCommandTests() 
            : base()
        {
        }

        [TestMethod]
        public void Mail_Commands_Valid()
        {
            this.ValidCommands.Add("MAIL FROM:<TEST@EXAMPLE.COM>");
            this.ValidCommands.Add("mail from:<test@example.com>");
            this.ValidCommands.Add("MAIL FROM:<test@example.com> BODY=7BIT");
            this.ValidCommands.Add("MAIL FROM:<test@example.com> BODY=8BITMIME");

            base.TestValidCommands();
        }

        [TestMethod]
        public void Mail_Commands_Invalid()
        {
            this.InvalidCommands.Add("MAIL FROM:test@example.com");
            this.InvalidCommands.Add("MAIL");
            this.InvalidCommands.Add("MAIL FROM:");
            this.InvalidCommands.Add("MAIL FROM:<TEST@EXAMPLE.COM> BODY=");
            this.InvalidCommands.Add("MAIL FROM:<TEST@EXAMPLE.COM>BODY=8BITMIME");
            this.InvalidCommands.Add("MAIL FROM:<TEST@EXAMPLE.COM> BODY=8BITMIME INVALIDARG=0");


            base.TestInvalidCommands();
        }
    }
}