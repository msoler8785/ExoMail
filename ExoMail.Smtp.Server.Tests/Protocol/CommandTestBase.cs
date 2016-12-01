using ExoMail.Smtp.Server.Configuration;
using ExoMail.Smtp.Server.Enums;
using ExoMail.Smtp.Server.Interfaces;
using ExoMail.Smtp.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace ExoMail.Smtp.Server.Protocol.Tests
{
    [TestClass()]
    public abstract class CommandTestBase<T>
    {
        public class TestUserIdentity : IUserIdentity
        {
            public List<string> AliasAddresses { get; set; }

            public string EmailAddress { get; set; }

            public string FirstName
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public List<string> Folders
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsActive
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string LastName
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public string MailboxPath
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public long MailboxSize
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public long MaxMailboxSize
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public long MaxMessageSize => ByteSizeHelper.FromMegaBytes(1);

            public long MessageCount
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string Password { get; set; }

            public string UserId
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public string UserName { get; set; }

            public TestUserIdentity()
            {
                AliasAddresses = new List<string>();
            }
        }

        public class TestUserStore : IUserStore
        {
            public string Domain => "example.com";
            private List<IUserIdentity> Identities { get; set; }

            public void AddUser(IUserIdentity userIdentity)
            {
                Identities.Add(userIdentity);
            }

            public List<IUserIdentity> GetIdentities()
            {
                return Identities;
            }

            public bool IsUserAuthenticated(string userName, string password)
            {
                return Identities.Any(x => x.UserName == userName && x.Password == password);
            }

            public bool IsValidRecipient(string emailAddress)
            {
                return Identities.Any(x => x.EmailAddress == emailAddress || x.AliasAddresses.Any(a => a == emailAddress));
            }

            public TestUserStore()
            {
                Identities = new List<IUserIdentity>();
            }
        }

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
                //Assert.IsTrue(command.ArgumentsValid);
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