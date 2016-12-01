using ExoMail.Smtp.Server.Authentication;
using ExoMail.Smtp.Server.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExoMail.Smtp.Server.Protocol.Tests
{
    [TestClass()]
    public class SmtpRcptCommandTests : CommandTestBase<SmtpRcptCommand>
    {
        public override SessionState TestSessionState => SessionState.RcptNeeded;

        public SmtpRcptCommandTests()
            : base()
        {
            var store = new TestUserStore();
            var user = new TestUserIdentity();
            user.EmailAddress = "user@example.com";
            user.AliasAddresses.Add("alias@example.com");

            store.AddUser(user);
            UserManager.GetUserManager.AddUserStore(store);
        }

        [TestMethod]
        public void Rcpt_Commands_Valid()
        {
            this.ValidCommands.Add("RCPT TO:<user@example.com>");
            this.ValidCommands.Add("RCPT TO:<alias@example.com>");
            this.ValidCommands.Add("RCPT TO:<alias@example.com> SIZE=1048576");

            base.TestValidCommands();
        }

        [TestMethod]
        public void Rcpt_Commands_Invalid()
        {
            this.InvalidCommands.Add("RCPT");
            this.InvalidCommands.Add("RCPT TO:");
            this.InvalidCommands.Add("RCPT TO:<>");
            this.InvalidCommands.Add("RCPT TO:<nonexistent@example.com>");
            this.InvalidCommands.Add("RCPT TO:<alias@example.com> SIZE=2048576");

            base.TestInvalidCommands();
        }
    }
}