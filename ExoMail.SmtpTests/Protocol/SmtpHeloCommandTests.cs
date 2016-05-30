﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class SmtpHeloCommandTests
    {
        private string _validCommand = "HELO host.example.com";
        private string _invalidCommand = "HELO host .example.com";

        private SmtpSession _session { get; set; }
        private SmtpCommandFactory _commandFactory { get; set; }
        public SmtpHeloCommandTests()
        {
            _session = new SmtpSession();
            _session.ServerConfig = MemoryConfig.Create();
            _session.SessionNetwork = new SmtpSessionNetwork(new TcpClient());

            _commandFactory = new SmtpCommandFactory(_session);
        }

        [TestMethod()]
        public void ValidSmtpHeloCommandTest()
        {
            var command = _commandFactory.Parse(_validCommand);
            var response = command.GetResponseAsync().Result;

            Assert.IsFalse(String.IsNullOrWhiteSpace(response));
            Assert.IsTrue(command.IsValid);
            Assert.IsTrue(command.ArgumentsValid);
            Assert.IsTrue(command.Arguments.Count() == 1);
            Assert.IsTrue(command.CommandType == SmtpCommandType.HELO);
            Assert.IsInstanceOfType(command, typeof(SmtpHeloCommand));
        }

        [TestMethod()]
        public void InvalidSmtpHeloCommandTest()
        {
            var command = _commandFactory.Parse(_invalidCommand);
            var response = command.GetResponseAsync().Result;

            Assert.IsFalse(String.IsNullOrWhiteSpace(response));
            Assert.IsFalse(command.IsValid);
            Assert.IsFalse(command.ArgumentsValid);
            Assert.IsFalse(command.Arguments.Count() == 1);
            Assert.IsTrue(command.CommandType == SmtpCommandType.HELO);
            Assert.IsInstanceOfType(command, typeof(SmtpHeloCommand));
        }
    }
}