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
    public abstract class CommandTestBase<T>
    {
        public List<string> ValidCommands { get; set; }
        public List<string> InvalidCommands { get; set; }
        public abstract SessionState TestSessionState { get; }
        public SmtpSession Session { get; set; }
        public SmtpCommandFactory CommandFactory { get; set; }

        public CommandTestBase()
        {
            Session = new SmtpSession();
            Session.ServerConfig = MemoryConfig.Create();
            Session.SessionNetwork = new SmtpSessionNetwork(new TcpClient());
            CommandFactory = new SmtpCommandFactory(Session);
            this.ValidCommands = new List<string>();
            this.InvalidCommands = new List<string>();
        }

        public virtual void TestValidCommands()
        {
            ValidCommandTests();
        }

        public virtual void TestInvalidCommands()
        {
            InvalidCommandTests();
        }
       
        private void ValidCommandTests()
        {
            Assert.IsFalse(this.ValidCommands.Count == 0);

            foreach (var validCommand in ValidCommands)
            {
                this.Session.SessionState = this.TestSessionState;
                var command = CommandFactory.Parse(validCommand);
                var response = command.GetResponseAsync().Result;

                Assert.IsFalse(String.IsNullOrWhiteSpace(response));
                Assert.IsTrue(command.IsValid);
                Assert.IsTrue(command.ArgumentsValid);
                Assert.IsInstanceOfType(command, typeof(T));
            }
        }

        private void InvalidCommandTests()
        {
            Assert.IsFalse(this.InvalidCommands.Count == 0);

            foreach (var invalidCommand in InvalidCommands)
            {
                this.Session.SessionState = this.TestSessionState;
                var command = CommandFactory.Parse(invalidCommand);
                var response = command.GetResponseAsync().Result;

                Assert.IsFalse(String.IsNullOrWhiteSpace(response));
                Assert.IsFalse(command.IsValid);
                Assert.IsInstanceOfType(command, typeof(T));
            }
        }
    }
}