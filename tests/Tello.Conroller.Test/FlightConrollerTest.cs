using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tello.Controller;
using Tello.Emulator.SDKV2;
using Tello.Messaging;

namespace Tello.Conroller.Test
{
    [TestClass]
    public class FlightConrollerTest
    {
        //[ClassInitialize]
        //public void ClassInitialize()
        //{

        //}

        //[TestInitialize]
        //public void TestInitialize()
        //{

        //}

        [TestMethod]
        public void FlightController_EnterSdkMode()
        {
            var tello = new TelloEmulator();
            var controller = new FlightController(tello, tello.StateServer, tello.VideoServer);

            var actualCommand = default(Commands);
            var actualResponse = String.Empty;
            controller.FlightControllerResponseReceived += delegate (object sender, FlightControllerResponseReceivedArgs e)
            {
                actualCommand = e.Command;
                actualResponse = e.Response;
            };

            tello.PowerOn();
            controller.EnterSdkMode();
            tello.PowerOff();

            Assert.AreEqual(Commands.EnterSdkMode, actualCommand);
            Assert.AreEqual("ok", actualResponse);
        }

        [TestMethod]
        public void FlightController_EnterSdkMode_PoweredOff()
        {
            var tello = new TelloEmulator();
            var controller = new FlightController(tello, tello.StateServer, tello.VideoServer);

            controller.FlightControllerExceptionThrown += delegate (object sender, FlightControllerExceptionThrownArgs e)
            {
                Assert.IsNotNull(e.Exception.InnerException);
                Assert.IsTrue(e.Exception.InnerException is TelloUnavailableException);
            };

            controller.FlightControllerCommandExceptionThrown += delegate (object sender, FlightControllerCommandExceptionThrownArgs e)
            {
                Assert.Fail("wrong event handler called");
            };

            controller.EnterSdkMode();
        }
    }
}
