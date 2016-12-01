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
    public class SmtpAuthCommandTests : CommandTestBase<SmtpAuthCommand>
    {
        public override SessionState TestSessionState => SessionState.RcptNeeded;

        public SmtpAuthCommandTests()
            : base()
        {
            var store = new TestUserStore();
            var user = new TestUserIdentity();
            user.UserName = "testuser";
            user.Password = "password";
            user.EmailAddress = "user@example.com";
            user.AliasAddresses.Add("alias@example.com");

            store.AddUser(user);
            UserManager.GetUserManager.AddUserStore(store);
        }

        [TestMethod]
        public void Auth_Commands_Valid()
        {
            this.ValidCommands.Add("AUTH PLAIN dGVzdHVzZXIAdGVzdHVzZXIAcGFzc3dvcmQ=");
        
            base.TestValidCommands();
        }

        [TestMethod]
        public void Auth_Commands_Invalid()
        {
            this.InvalidCommands.Add("AUTH");

            base.TestInvalidCommands();
        }
    }
}