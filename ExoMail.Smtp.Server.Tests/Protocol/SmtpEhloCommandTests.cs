using ExoMail.Smtp.Server.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExoMail.Smtp.Server.Protocol.Tests
{
    [TestClass()]
    public class SmtpEhloCommandTests : CommandTestBase<SmtpEhloCommand>
    {
        public override SessionState TestSessionState => SessionState.EhloNeeded;

        public SmtpEhloCommandTests()
            : base()
        {
        }

        [TestMethod]
        public void Ehlo_Commands_Valid()
        {
            this.ValidCommands.Add("EHLO host.example.com");
            this.ValidCommands.Add("EHLO host");

            base.TestValidCommands();
        }

        [TestMethod]
        public void Ehlo_Commands_Invalid()
        {
            this.InvalidCommands.Add("EHLO host .example.com");
            this.InvalidCommands.Add("EHLO");

            base.TestInvalidCommands();
        }
    }
}