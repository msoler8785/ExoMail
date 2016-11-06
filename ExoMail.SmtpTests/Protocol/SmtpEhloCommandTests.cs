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