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
using ExoMail.Smtp.Authentication;
using ExoMail.Smtp.Interfaces;

namespace ExoMail.Smtp.Protocol.Tests
{
    [TestClass()]
    public class SmtpRcptCommandTests : CommandTestBase<SmtpRcptCommand>
    {
        private class TestUserIdentity : IUserIdentity
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

        private class TestUserStore : IUserStore
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