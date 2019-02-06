using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tello.Controller;
using Tello.Emulator.SDKV2;

namespace Tello.Conroller.Test
{
    [TestClass]
    public class MotorTimeInSeconds
    {
        [ClassInitialize]
        public void ClassInitialize()
        {

        }

        [TestInitialize]
        public void TestInitialize()
        {

        }


        [TestMethod]
        public void FlightController_TakeOff()
        {
            var tello = new TelloEmulator();
            var controller = new FlightController(tello, tello.StateServer, tello.VideoServer);
            tello.PowerOn();

            controller.TakeOff();


            tello.PowerOff();
        }
    }
}
