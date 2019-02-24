using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;
using Tello.Emulator.SDKV2;
using Tello.Messaging;
using static Tello.Controller.FlightController;

namespace Tello.Controller.Test
{
    [TestClass]
    public class FlightControllerTest
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
            controller.TelloControllerResponseReceived += delegate (object sender, TelloControllerResponseReceivedArgs e)
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

            controller.TelloControllerExceptionThrown += delegate (object sender, TelloControllerExceptionThrownArgs e)
            {
                Assert.IsNotNull(e.Exception.InnerException);
                Assert.IsTrue(e.Exception.InnerException is TelloUnavailableException);
            };

            controller.TelloControllerCommandExceptionThrown += delegate (object sender, FlightControllerCommandExceptionThrownArgs e)
            {
                Assert.Fail("wrong event handler called");
            };

            controller.EnterSdkMode();
        }

        private void AwaitCommand(FlightController controller, Action command)
        {
            var isOk = false;
            void handler(object sender, TelloControllerResponseReceivedArgs e)
            {
                isOk = controller.IsResponseOk(e.Command, e.Response);
            }
            controller.TelloControllerResponseReceived += handler;
            command?.Invoke();
            var wait = new SpinWait();
            while (isOk == false)
            {
                wait.SpinOnce();
            }
            controller.TelloControllerResponseReceived -= handler;
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
                controller.TelloControllerResponseReceived += delegate (object sender, TelloControllerResponseReceivedArgs e)
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
                AwaitCommand(controller, controller.TakeOff);
                Assert.AreEqual(FlightStates.InFlight, controller.FlightState);

                controller.TelloControllerResponseReceived += delegate (object sender, TelloControllerResponseReceivedArgs e)
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

        private TelloControllerResponseReceivedArgs TestMovement(TelloEmulator tello, FlightController controller, Action<int> command, int arg)
        {
            var result = default(TelloControllerResponseReceivedArgs);

            tello.PowerOn();
            try
            {
                controller.EnterSdkMode();
                AwaitCommand(controller, controller.TakeOff);
                Assert.AreEqual(FlightStates.InFlight, controller.FlightState);

                controller.TelloControllerResponseReceived += delegate (object sender, TelloControllerResponseReceivedArgs e)
                {
                    result = e;
                };
                command?.Invoke(arg);
                var wait = new SpinWait();
                var clock = Stopwatch.StartNew();
                while (result == default(TelloControllerResponseReceivedArgs) && clock.ElapsedMilliseconds < 5000)
                {
                    wait.SpinOnce();
                }
            }
            finally
            {
                tello.PowerOff();
            }
            return result;
        }

        [TestMethod]
        public void FlightController_Up()
        {
            var tello = new TelloEmulator();
            var controller = new FlightController(tello, tello.StateServer, tello.VideoServer);
            var e = TestMovement(tello, controller, controller.GoUp, 20);

            Assert.IsNotNull(e);
            Assert.AreEqual(Commands.Up, e.Command);
            Assert.AreEqual("ok", e.Response);
            Assert.AreEqual(FlightStates.InFlight, controller.FlightState);
        }

        [TestMethod]
        public void FlightController_Down()
        {
            var tello = new TelloEmulator();
            var controller = new FlightController(tello, tello.StateServer, tello.VideoServer);
            var e = TestMovement(tello, controller, controller.GoDown, 20);

            Assert.IsNotNull(e);
            Assert.AreEqual(Commands.Down, e.Command);
            Assert.AreEqual("ok", e.Response);
            Assert.AreEqual(FlightStates.InFlight, controller.FlightState);
        }

        [TestMethod]
        public void FlightController_Left()
        {
            var tello = new TelloEmulator();
            var controller = new FlightController(tello, tello.StateServer, tello.VideoServer);
            var e = TestMovement(tello, controller, controller.GoLeft, 20);

            Assert.IsNotNull(e);
            Assert.AreEqual(Commands.Left, e.Command);
            Assert.AreEqual("ok", e.Response);
            Assert.AreEqual(FlightStates.InFlight, controller.FlightState);
        }

        [TestMethod]
        public void FlightController_Right()
        {
            var tello = new TelloEmulator();
            var controller = new FlightController(tello, tello.StateServer, tello.VideoServer);
            var e = TestMovement(tello, controller, controller.GoRight, 20);

            Assert.IsNotNull(e);
            Assert.AreEqual(Commands.Right, e.Command);
            Assert.AreEqual("ok", e.Response);
            Assert.AreEqual(FlightStates.InFlight, controller.FlightState);
        }

        [TestMethod]
        public void FlightController_Forward()
        {
            var tello = new TelloEmulator();
            var controller = new FlightController(tello, tello.StateServer, tello.VideoServer);
            var e = TestMovement(tello, controller, controller.GoForward, 20);

            Assert.IsNotNull(e);
            Assert.AreEqual(Commands.Forward, e.Command);
            Assert.AreEqual("ok", e.Response);
            Assert.AreEqual(FlightStates.InFlight, controller.FlightState);
        }

        [TestMethod]
        public void FlightController_Back()
        {
            var tello = new TelloEmulator();
            var controller = new FlightController(tello, tello.StateServer, tello.VideoServer);
            var e = TestMovement(tello, controller, controller.GoBackward, 20);

            Assert.IsNotNull(e);
            Assert.AreEqual(Commands.Back, e.Command);
            Assert.AreEqual("ok", e.Response);
            Assert.AreEqual(FlightStates.InFlight, controller.FlightState);
        }

        [TestMethod]
        public void FlightController_Clockwise()
        {
            var tello = new TelloEmulator();
            var controller = new FlightController(tello, tello.StateServer, tello.VideoServer);
            var e = TestMovement(tello, controller, controller.TurnClockwise, 20);

            Assert.IsNotNull(e);
            Assert.AreEqual(Commands.ClockwiseTurn, e.Command);
            Assert.AreEqual("ok", e.Response);
            Assert.AreEqual(FlightStates.InFlight, controller.FlightState);
        }

        [TestMethod]
        public void FlightController_CounterClockwise()
        {
            var tello = new TelloEmulator();
            var controller = new FlightController(tello, tello.StateServer, tello.VideoServer);
            var e = TestMovement(tello, controller, controller.TurnCounterClockwise, 20);

            Assert.IsNotNull(e);
            Assert.AreEqual(Commands.CounterClockwiseTurn, e.Command);
            Assert.AreEqual("ok", e.Response);
            Assert.AreEqual(FlightStates.InFlight, controller.FlightState);
        }

        [TestMethod]
        public void FlightController_TurnRight()
        {
            var tello = new TelloEmulator();
            var controller = new FlightController(tello, tello.StateServer, tello.VideoServer);
            var e = TestMovement(tello, controller, controller.TurnRight, 20);

            Assert.IsNotNull(e);
            Assert.AreEqual(Commands.ClockwiseTurn, e.Command);
            Assert.AreEqual("ok", e.Response);
            Assert.AreEqual(FlightStates.InFlight, controller.FlightState);
        }

        [TestMethod]
        public void FlightController_TurnLeft()
        {
            var tello = new TelloEmulator();
            var controller = new FlightController(tello, tello.StateServer, tello.VideoServer);
            var e = TestMovement(tello, controller, controller.TurnLeft, 20);

            Assert.IsNotNull(e);
            Assert.AreEqual(Commands.CounterClockwiseTurn, e.Command);
            Assert.AreEqual("ok", e.Response);
            Assert.AreEqual(FlightStates.InFlight, controller.FlightState);
        }
    }
}
