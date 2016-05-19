using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExoMail.Smtp.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExoMail.Smtp.Enums;

namespace ExoMail.Smtp.Protocol.Tests
{
    [TestClass()]
    public class SmtpHeloCommandTests
    {
        SmtpSession testSession = new SmtpSession()
        {
            SessionState = SessionState.EhloNeeded
        };
        string heloCommand = "HELO mail.example.com";
        string invalidDomain = "HELO _mail.example.com";
        string invalidArgs = "HELO mail.example.com foo";
        string invalidCommand = "XELO mail.example.com";

        [TestMethod()]
        public void SmtpHeloCommandTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetResponseAsyncTest()
        {
            Assert.Fail();
        }
    }
}