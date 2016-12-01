using ExoMail.Smtp.Server.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExoMail.Smtp.Server.Protocol.Tests
{
    [TestClass()]
    public class SmtpHeloCommandTests : CommandTestBase<SmtpHeloCommand>
    {
        public override SessionState TestSessionState => SessionState.EhloNeeded;

        public SmtpHeloCommandTests()
            : base()
        {
        }

        [TestMethod]
        public void Helo_Commands_Valid()
        {
            this.ValidCommands.Add("HELO host.example.com");
            this.ValidCommands.Add("HELO host");

            base.TestValidCommands();
        }

        [TestMethod]
        public void Helo_Commands_Invalid()
        {
            this.InvalidCommands.Add("HELO host .example.com");
            this.InvalidCommands.Add("HELO");

            base.TestInvalidCommands();
        }
    }
}