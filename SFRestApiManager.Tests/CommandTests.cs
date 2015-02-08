using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using SFRestApiUpdater.Commands;

namespace SFRestApiUpdater.Tests
{
    [TestClass]
    public class CommandTests
    {
        [TestMethod]
        public void TestArgumentsObjects()
        {
            String argument1Name = "-I";
            String argument1Value = "1";
            String argument2Name = "-N";
            String argument2Value = "John Doe";
            String[] argumentList = new String[2];
            argumentList[0] = String.Format("{0}={1}", argument1Name, argument1Value);
            argumentList[1] = String.Format("{0}={1}", argument2Name, argument2Value);

            Arguments actualArguments = new Arguments(argumentList);
            Assert.AreEqual(argument1Value, actualArguments["I"]);
            Assert.AreEqual(argument2Value, actualArguments["N"]);
        }

        [TestMethod]
        public void TestStatusCallIsNotValid()
        {
            String[] argumentList = DefaultArgumentList();
            argumentList[0] = "-S=";
            StatusCallCommands commands = new StatusCallCommands(new Arguments(argumentList));
            Assert.AreEqual(false, commands.IsValid());
        }

        [TestMethod]
        public void TestStatusCallIsValid()
        {
            var argumentList = DefaultArgumentList();
            StatusCallCommands commands = new StatusCallCommands(new Arguments(argumentList));
            Assert.AreEqual(true, commands.IsValid());
        }

        [TestMethod]
        public void TestStatusCallConstructor()
        {
            var argumentList = DefaultArgumentList();
            StatusCallCommands commands = new StatusCallCommands(new Arguments(argumentList));
            Assert.AreEqual("999999", commands.Spon);
            Assert.AreEqual(99, commands.Status);
            Assert.AreEqual(false, commands.IsTest);
            Assert.AreEqual("www.approvalURL.com", commands.ApprovalURL);
            Assert.AreEqual(String.Empty, commands.FreelancerId);
            Assert.AreEqual(String.Empty, commands.TaskId);
            Assert.AreEqual(String.Empty, commands.OrderId);
            Assert.AreEqual(String.Empty, commands.Role);
            Assert.AreEqual(true, commands.ProcessOnInsert);

        }

        private String[] DefaultArgumentList()
        {
            String[] argumentList = new String[8];
            argumentList[0] = String.Format("-S={0}", 999999);
            argumentList[1] = String.Format("-T={0}", 99);
            argumentList[2] = String.Format("-TE={0}", false);
            argumentList[3] = String.Format("-U={0}", "www.approvalURL.com");
            argumentList[4] = String.Format("-FI={0}", "");
            argumentList[5] = String.Format("-TI={0}", "");
            argumentList[6] = String.Format("-O={0}", "");
            argumentList[7] = String.Format("-R={0}", "");
            return argumentList;
        }
    }
}
