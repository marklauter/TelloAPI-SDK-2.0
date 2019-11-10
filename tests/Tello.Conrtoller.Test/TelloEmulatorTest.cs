// <copyright file="TelloEmulatorTest.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
//namespace Tello.Controller.Test
//{
//    [TestClass]
//    public class TelloEmulatorTest
//    {
//        [TestMethod]
//        public void TelloEmulator_CTOR_Test()
//        {
//            var tello = new TelloEmulator();
//            Assert.AreEqual(MessengerStates.Disconnected, tello.MessengerState);
//            Assert.IsNotNull(tello.StateServer);
//            Assert.AreEqual(ReceiverStates.Stopped, tello.StateServer.State);
//            Assert.IsNotNull(tello.VideoServer);
//            Assert.AreEqual(ReceiverStates.Stopped, tello.VideoServer.State);
//            Assert.IsNotNull(tello.Position);
//            Assert.AreEqual(0, tello.Position.Heading);
//            Assert.AreEqual(0, tello.Position.X);
//            Assert.AreEqual(0, tello.Position.Y);
//            Assert.AreEqual(0, tello.Position.Height);
//        }

//        [TestMethod]
//        public void TelloEmulator_Power_Test()
//        {
//            var tello = new TelloEmulator();

//            tello.PowerOn();
//            Assert.IsTrue(tello.IsPoweredUp);

//            tello.PowerOff();
//            Assert.IsFalse(tello.IsPoweredUp);
//        }

//        [TestMethod]
//        public void TelloEmulator_Connection_Test()
//        {
//            var tello = new TelloEmulator();
//            tello.PowerOn();
//            try
//            {
//                tello.Connect();
//                Assert.AreEqual(MessengerStates.Connected, tello.MessengerState);

//                tello.Disconnect();
//                Assert.AreEqual(MessengerStates.Disconnected, tello.MessengerState);
//            }
//            finally
//            {
//                tello.PowerOff();
//            }
//        }

//        [TestMethod]
//        public async Task TelloEmulator_SendAsync_Test()
//        {
//            var tello = new TelloEmulator();

//            tello.PowerOn();
//            try
//            {
//                tello.Connect();
//                try
//                {
//                    var bytes = Encoding.UTF8.GetBytes("command");
//                    var request = Request.FromData(bytes);
//                    var response = await tello.SendAsync(request);
//                    Assert.IsTrue(response.Success);
//                    Assert.AreEqual(request.Id, response.Request);
//                    Assert.IsNull(response.Exception);
//                    var message = Encoding.UTF8.GetString(response.Data);
//                    Assert.AreEqual("ok", message.ToLowerInvariant());
//                }
//                finally
//                {
//                    tello.Disconnect();
//                }
//            }
//            finally
//            {
//                tello.PowerOff();
//            }
//        }

//        [TestMethod]
//        public async Task TelloEmulator_SendAsync_NotConnected_Test()
//        {
//            var tello = new TelloEmulator();

//            tello.PowerOn();
//            try
//            {
//                var bytes = Encoding.UTF8.GetBytes("dummy");
//                var request = Request.FromData(bytes);
//                var response = await tello.SendAsync(request);
//                Assert.IsFalse(response.Success);
//                Assert.AreEqual(request.Id, response.Request);
//                Assert.IsNotNull(response.Exception);
//                Assert.IsTrue(response.Exception is InvalidOperationException);
//            }
//            finally
//            {
//                tello.PowerOff();
//            }
//        }

//        [TestMethod]
//        public async Task TelloEmulator_SendAsync_BadCommand_Test()
//        {
//            var tello = new TelloEmulator();

//            tello.PowerOn();
//            try
//            {
//                tello.Connect();
//                try
//                {
//                    var bytes = Encoding.UTF8.GetBytes("dummy");
//                    var request = Request.FromData(bytes);
//                    var response = await tello.SendAsync(request);
//                    Assert.IsTrue(response.Success);
//                    Assert.AreEqual(request.Id, response.Request);
//                    Assert.IsNull(response.Exception);
//                    var message = Encoding.UTF8.GetString(response.Data);
//                    Assert.AreEqual("error", message.ToLowerInvariant());
//                }
//                finally
//                {
//                    tello.Disconnect();
//                }
//            }
//            finally
//            {
//                tello.PowerOff();
//            }
//        }
//    }
//}

