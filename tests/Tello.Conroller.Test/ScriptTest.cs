using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tello.Messaging;
using Tello.Scripting;

namespace Tello.Controller.Test
{
    [TestClass]
    public class ScriptTest
    {
        [TestMethod]
        public void ScriptBuilder_Test()
        {
            var builder = new ScriptBuilder();
            var takeoff = builder.AddToken("takeoff", Commands.Takeoff);
            Assert.AreEqual("takeoff", takeoff.Id);
            Assert.AreEqual(Commands.Takeoff, takeoff.Command);
            Assert.AreEqual(1, takeoff.Order);

            var forward = builder.AddToken("forward", Commands.Forward, 20);
            Assert.AreEqual("forward", forward.Id);
            Assert.AreEqual(Commands.Forward, forward.Command);
            Assert.IsNotNull(forward.Args);
            Assert.AreEqual(1, forward.Args.Length);
            Assert.AreEqual(20, forward.Args[0]);
            Assert.AreEqual(2, forward.Order);

            var backward = builder.AddToken("backward", Commands.Back, 20);
            Assert.AreEqual("backward", backward.Id);
            Assert.AreEqual(Commands.Back, backward.Command);
            Assert.IsNotNull(backward.Args);
            Assert.AreEqual(1, backward.Args.Length);
            Assert.AreEqual(20, backward.Args[0]);
            Assert.AreEqual(3, backward.Order);

            builder.MoveRight("backward");
            Assert.AreEqual(3, backward.Order);

            builder.MoveLeft("backward");
            Assert.AreEqual(2, backward.Order);
            Assert.AreEqual(3, forward.Order);

            var json = builder.ToJson();
            Assert.IsFalse(string.IsNullOrEmpty(json));
        }

        [TestMethod]
        public void CommandScript_Test()
        {
            var builder = new ScriptBuilder();
            var takeoff = builder.AddToken("takeoff", Commands.Takeoff);
            var forward = builder.AddToken("forward", Commands.Forward, 20);
            var backward = builder.AddToken("backward", Commands.Back, 20);

            var json = builder.ToJson();

            var script = TelloScript.FromJson(json);
            Assert.AreEqual(3, script._tokens.Length);
        }
    }
}