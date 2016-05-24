using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExoMail.Smtp.Protocol.Tests
{
    [TestClass()]
    public class SmtpCommandFactoryTests
    {
        private SmtpCommandFactory factory { get; set; }
        private SmtpSession smtpSession { get; set; }

        private string _heloCommand = "HELO mail.example.com";
        private string _ehloCommand = "EHLO mail.example.com";
        private string _mailCommand = "MAIL FROM:<fbar@example.com>";
        private string _rcptCommand = "RCPT TO:<fbar@example.net>";
        private string _helpCommand = "HELP";
        private string _noopCommand = "NOOP";
        private string _quitCommand = "QUIT";
        private string _startTlsCommand = "STARTTLS";
        private string _dataCommand = "DATA";
        private string _rsetCommand = "RSET";
        private string _turnCommand = "TURN";
        private string _vrfyCommand = "VRFY";

        public SmtpCommandFactoryTests()
        {
            smtpSession = new SmtpSession();
            factory = new SmtpCommandFactory(smtpSession);
        }

        [TestMethod()]
        public void HeloCommandTypeTest()
        {
            var command = factory.Parse(this._heloCommand);
            Assert.IsInstanceOfType(command, typeof(SmtpHeloCommand));
        }

        [TestMethod]
        public void EhloCommandTypeTest()
        {
            var command = factory.Parse(this._ehloCommand);
            Assert.IsInstanceOfType(command, typeof(SmtpEhloCommand));
        }

        [TestMethod]
        public void MailCommandTypeTest()
        {
            var command = factory.Parse(this._mailCommand);
            Assert.IsInstanceOfType(command, typeof(SmtpMailCommand));
        }

        [TestMethod]
        public void RcptCommandTypeTest()
        {
            var command = factory.Parse(this._rcptCommand);
            Assert.IsInstanceOfType(command, typeof(SmtpRcptCommand));
        }

        [TestMethod]
        public void DataCommandTypeTest()
        {
            var command = factory.Parse(this._dataCommand);
            Assert.IsInstanceOfType(command, typeof(SmtpDataCommand));
        }

        [TestMethod]
        public void RsetCommandTypeTest()
        {
            var command = factory.Parse(this._rsetCommand);
            Assert.IsInstanceOfType(command, typeof(SmtpRsetCommand));
        }

        [TestMethod]
        public void HelpCommandTypeTest()
        {
            var command = factory.Parse(this._helpCommand);
            Assert.IsInstanceOfType(command, typeof(SmtpHelpCommand));
        }

        [TestMethod]
        public void NoopCommandTypeTest()
        {
            var command = factory.Parse(this._noopCommand);
            Assert.IsInstanceOfType(command, typeof(SmtpNoopCommand));
        }

        [TestMethod]
        public void QuitCommandTypeTest()
        {
            var command = factory.Parse(this._quitCommand);
            Assert.IsInstanceOfType(command, typeof(SmtpQuitCommand));
        }

        [TestMethod]
        public void StartTlsCommandTypeTest()
        {
            var command = factory.Parse(this._startTlsCommand);
            Assert.IsInstanceOfType(command, typeof(SmtpStartTlsCommand));
        }

        [TestMethod]
        public void TurnCommandTypeTest()
        {
            var command = factory.Parse(this._turnCommand);
            Assert.IsInstanceOfType(command, typeof(SmtpTurnCommand));
        }

        [TestMethod]
        public void VrfyCommandTypeTest()
        {
            var command = factory.Parse(this._vrfyCommand);
            Assert.IsInstanceOfType(command, typeof(SmtpVrfyCommand));
        }
    }
}