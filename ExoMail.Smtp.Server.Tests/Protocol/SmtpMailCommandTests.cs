using ExoMail.Smtp.Server.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExoMail.Smtp.Server.Protocol.Tests
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