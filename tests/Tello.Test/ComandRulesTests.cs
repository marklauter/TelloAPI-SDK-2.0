using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Tello.Test
{
    [TestClass]
    public class CommandRulesTests
    {
        [TestMethod]
        public void CommandRules_Rules_String_Method_returns_correct_rule()
        {
            var rule = CommandRules.Rules("forward");
            Assert.IsNotNull(rule);
            Assert.AreEqual(Commands.Forward, rule.Command);

            rule = CommandRules.Rules("command");
            Assert.IsNotNull(rule);
            Assert.AreEqual(Commands.EnterSdkMode, rule.Command);
        }

        [TestMethod]
        public void CommandRules_Rules_String_Method_throws_on_bad_argument()
        {
            Assert.ThrowsException<KeyNotFoundException>(() => CommandRules.Rules("boo"));
        }
    }
}
