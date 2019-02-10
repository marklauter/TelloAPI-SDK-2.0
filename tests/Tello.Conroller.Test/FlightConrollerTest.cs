using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using Tello.Controller;
using Tello.Emulator.SDKV2;
using Tello.Messaging;
using static Tello.Controller.FlightController;

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
        public void FlightController_CTOR()
        {
            var tello = new TelloEmulator();
            var controller = new FlightController(tello, tello.StateServer, tello.VideoServer);
            Assert.AreEqual(ConnectionStates.Disconnected, controller.ConnectionState);
            Assert.AreEqual(FlightStates.StandingBy, controller.FlightState);
            Assert.AreEqual(VideoStates.Stopped, controller.VideoState);
            Assert.AreEqual(0, controller.ReportedSpeed);
            Assert.AreEqual(0, controller.ReportedBattery);
            Assert.AreEqual(0, controller.ReportedTime);
        }

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
            try
            {
                controller.EnterSdkMode();
                Assert.AreEqual(ConnectionStates.CommandLinkEstablished, controller.ConnectionState);
            }
            finally
            {
                tello.PowerOff();
            }

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

        [TestMethod]
        public void FlightController_TakeOff()
        {
            var tello = new TelloEmulator();
            var controller = new FlightController(tello, tello.StateServer, tello.VideoServer);

            var actualCommand = default(Commands);
            var actualResponse = String.Empty;

            tello.PowerOn();
            try
            {
                controller.EnterSdkMode();
                controller.FlightControllerResponseReceived += delegate (object sender, FlightControllerResponseReceivedArgs e)
                {
                    actualCommand = e.Command;
                    actualResponse = e.Response;
                };
                controller.TakeOff();
                var wait = new SpinWait();
                while (actualCommand == default(Commands))
                {
                    wait.SpinOnce();
                }
            }
            finally
            {
                tello.PowerOff();
            }

            Assert.AreEqual(Commands.Takeoff, actualCommand);
            Assert.AreEqual("ok", actualResponse);
            Assert.AreEqual(FlightStates.InFlight, controller.FlightState);
        }

        [TestMethod]
        public void FlightController_Land()
        {
            var tello = new TelloEmulator();
            var controller = new FlightController(tello, tello.StateServer, tello.VideoServer);

            var actualCommand = default(Commands);
            var actualResponse = String.Empty;

            tello.PowerOn();
            try
            {
                controller.EnterSdkMode();
                controller.FlightControllerResponseReceived += delegate (object sender, FlightControllerResponseReceivedArgs e)
                {
                    actualCommand = e.Command;
                    actualResponse = e.Response;
                };
                controller.Land();
                var wait = new SpinWait();
                while (actualCommand == default(Commands))
                {
                    wait.SpinOnce();
                }
            }
            finally
            {
                tello.PowerOff();
            }

            Assert.AreEqual(Commands.Land, actualCommand);
            Assert.AreEqual("ok", actualResponse);
            Assert.AreEqual(FlightStates.StandingBy, controller.FlightState);
        }
    }
}
