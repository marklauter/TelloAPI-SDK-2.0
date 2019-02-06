using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tello.Controller;
using Tello.Emulator.SDKV2;
using Tello.Messaging;

namespace Tello.Conroller.Test
{
    [TestClass]
    public class MotorTimeInSeconds
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
            var actualResponse = string.Empty;
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

            var actualCommand = default(Commands);
            var actualResponse = string.Empty;
            
            controller.FlightControllerResponseReceived += delegate (object sender, FlightControllerResponseReceivedArgs e)
            {
                actualCommand = e.Command;
                actualResponse = e.Response;
            };

            controller.FlightControllerExceptionThrown += delegate (object sender, FlightControllerExceptionThrownArgs e)
            {

            };

            controller.FlightControllerCommandExceptionThrown += delegate (object sender, FlightControllerCommandExceptionThrownArgs e)
            {
                //todo: this is throwing the wrong error - too tired to fix it right now
            };

            controller.EnterSdkMode();

            Assert.AreEqual(Commands.Unknown, actualCommand);
            Assert.AreEqual(string.Empty, actualResponse);
        }
    }
}
