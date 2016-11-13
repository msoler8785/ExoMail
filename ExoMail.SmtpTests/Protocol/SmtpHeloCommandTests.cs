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