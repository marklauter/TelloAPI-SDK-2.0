// <copyright file="TelloObservationsSqliteTests.Observations.Sqlite.Tests.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Messenger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using Repository.Sqlite;
using Tello.Messaging;
using Tello.State;

namespace Tello.Entities.Sqlite.Test
{
    [TestClass]
    public class TelloObservationsSqliteTests
    {
        private IRepository CreateRepository(bool deleteFile = true, [CallerMemberName]string callerName = null)
        {
            var path = Path.GetTempPath();
            var fileName = $"{callerName}.sqlite";
            var fullPath = Path.Combine(path, fileName);

            if (deleteFile && File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            var repo = new SqliteRepository((path, fileName));
            Assert.IsTrue(File.Exists(fullPath));
            return repo;
        }

        [TestMethod]
        public void Session_NewEntity_Insert_Update_Read()
        {
            using (var repo = this.CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<Session>());

                var start = DateTime.UtcNow;
                var session = repo.NewEntity<Session>(start);
                Assert.IsNotNull(session);
                Assert.AreEqual(1, session.Id);
                Assert.AreEqual(start, session.StartTime);
                Assert.AreEqual(0, session.Duration.TotalMilliseconds);

                session.Duration = TimeSpan.FromSeconds(1);
                Assert.AreEqual(1, repo.Update(session));

                session = repo.Read<Session>(1);
                Assert.IsNotNull(session);
                Assert.AreEqual(1, session.Id);
                Assert.AreEqual(start, session.StartTime);
                Assert.AreEqual(1000, session.Duration.TotalMilliseconds);
            }
        }

        [TestMethod]
        public void ObservationGroup_NewEntity_Insert_Update_Read()
        {
            using (var repo = this.CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<Session>());
                Assert.IsTrue(repo.CreateCatalog<ObservationGroup>());

                var start = DateTime.UtcNow;
                var session = repo.NewEntity<Session>(start, TimeSpan.FromSeconds(1));
                Assert.IsNotNull(session);
                Assert.AreEqual(1, session.Id);
                Assert.AreEqual(start, session.StartTime);
                Assert.AreEqual(1000, session.Duration.TotalMilliseconds);

                var group = repo.NewEntity<ObservationGroup>(session, start);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);

                group = repo.Read<ObservationGroup>(1);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);
            }
        }

        [TestMethod]
        public void TelloCommandObservation_Insert_Read()
        {
            using (var repo = this.CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<Session>());
                Assert.IsTrue(repo.CreateCatalog<ObservationGroup>());
                Assert.IsTrue(repo.CreateCatalog<ResponseObservation>());

                var start = DateTime.UtcNow;
                var session = repo.NewEntity<Session>(start, TimeSpan.FromSeconds(1));
                Assert.IsNotNull(session);
                Assert.AreEqual(1, session.Id);
                Assert.AreEqual(start, session.StartTime);
                Assert.AreEqual(1000, session.Duration.TotalMilliseconds);

                var group = repo.NewEntity<ObservationGroup>(session, start);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);

                group = repo.Read<ObservationGroup>(1);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);

                var request = new TelloRequest(Commands.Takeoff);
                var ok = "ok";
                var okBytes = Encoding.UTF8.GetBytes(ok);
                IResponse<string> response = new TelloResponse(request, okBytes, TimeSpan.FromSeconds(1));

                var observation = new ResponseObservation(group, response);
                Assert.AreEqual(1, repo.Insert(observation));

                observation = repo.Read<ResponseObservation>(1);
                Assert.IsNotNull(observation);
                Assert.AreEqual(1, observation.Id);
                Assert.AreEqual(1, observation.GroupId);
                Assert.AreEqual(request.Timestamp, observation.TimeInitiated);
                Assert.AreEqual(Commands.Takeoff, observation.Command.Rule.Command);
                Assert.AreEqual("ok", observation.Response);
            }
        }

        [TestMethod]
        public void PositionObservation_Insert_Read()
        {
            using (var repo = this.CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<Session>());
                Assert.IsTrue(repo.CreateCatalog<ObservationGroup>());
                Assert.IsTrue(repo.CreateCatalog<PositionObservation>());

                var start = DateTime.UtcNow;
                var session = repo.NewEntity<Session>(start, TimeSpan.FromSeconds(1));
                Assert.IsNotNull(session);
                Assert.AreEqual(1, session.Id);
                Assert.AreEqual(start, session.StartTime);
                Assert.AreEqual(1000, session.Duration.TotalMilliseconds);

                var group = repo.NewEntity<ObservationGroup>(session, start);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);

                group = repo.Read<ObservationGroup>(1);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);

                var stateString = "mid:-1;x:0;y:0;z:0;mpry:0,0,0;pitch:1;roll:2;yaw:3;vgx:4;vgy:5;vgz:6;templ:7;temph:8;tof:9;h:10;bat:11;baro:12.13;time:14;agx:15.16;agy:17.18;agz:19.20;";
                var state = new TelloState(stateString);

                var observation = new PositionObservation(group, state);
                Assert.AreEqual(1, repo.Insert(observation));

                observation = repo.Read<PositionObservation>(1);
                Assert.IsNotNull(observation);
                Assert.AreEqual(1, observation.Id);
                Assert.AreEqual(state.Timestamp, observation.Timestamp);
                Assert.AreEqual(1, observation.GroupId);
                Assert.AreEqual(10, observation.AltitudeAGLInCm);
                Assert.AreEqual(12.13, observation.BarometricPressueInCm);
                Assert.AreEqual(0, observation.Heading);
                Assert.AreEqual(0, observation.X);
                Assert.AreEqual(0, observation.Y);
            }
        }

        [TestMethod]
        public void AirSpeed_Insert_Read()
        {
            using (var repo = this.CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<Session>());
                Assert.IsTrue(repo.CreateCatalog<ObservationGroup>());
                Assert.IsTrue(repo.CreateCatalog<AirSpeedObservation>());

                var start = DateTime.UtcNow;
                var session = repo.NewEntity<Session>(start, TimeSpan.FromSeconds(1));
                Assert.IsNotNull(session);
                Assert.AreEqual(1, session.Id);
                Assert.AreEqual(start, session.StartTime);
                Assert.AreEqual(1000, session.Duration.TotalMilliseconds);

                var group = repo.NewEntity<ObservationGroup>(session, start);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);

                group = repo.Read<ObservationGroup>(1);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);

                var stateString = "mid:-1;x:0;y:0;z:0;mpry:0,0,0;pitch:1;roll:2;yaw:3;vgx:4;vgy:5;vgz:6;templ:7;temph:8;tof:9;h:10;bat:11;baro:12.13;time:14;agx:15.16;agy:17.18;agz:19.20;";
                var state = new TelloState(stateString);

                var observation = new AirSpeedObservation(group, state);
                Assert.AreEqual(1, repo.Insert(observation));

                observation = repo.Read<AirSpeedObservation>(1);
                Assert.IsNotNull(observation);
                Assert.AreEqual(1, observation.Id);
                Assert.AreEqual(state.Timestamp, observation.Timestamp);
                Assert.AreEqual(1, observation.GroupId);
                Assert.AreEqual(15.16, observation.AccelerationX);
                Assert.AreEqual(17.18, observation.AccelerationY);
                Assert.AreEqual(19.20, observation.AccelerationZ);
                Assert.AreEqual(4, observation.SpeedX);
                Assert.AreEqual(5, observation.SpeedY);
                Assert.AreEqual(6, observation.SpeedZ);
            }
        }

        [TestMethod]
        public void Attitude_Insert_Read()
        {
            using (var repo = this.CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<Session>());
                Assert.IsTrue(repo.CreateCatalog<ObservationGroup>());
                Assert.IsTrue(repo.CreateCatalog<AttitudeObservation>());

                var start = DateTime.UtcNow;
                var session = repo.NewEntity<Session>(start, TimeSpan.FromSeconds(1));
                Assert.IsNotNull(session);
                Assert.AreEqual(1, session.Id);
                Assert.AreEqual(start, session.StartTime);
                Assert.AreEqual(1000, session.Duration.TotalMilliseconds);

                var group = repo.NewEntity<ObservationGroup>(session, start);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);

                group = repo.Read<ObservationGroup>(1);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);

                var stateString = "mid:-1;x:0;y:0;z:0;mpry:0,0,0;pitch:1;roll:2;yaw:3;vgx:4;vgy:5;vgz:6;templ:7;temph:8;tof:9;h:10;bat:11;baro:12.13;time:14;agx:15.16;agy:17.18;agz:19.20;";
                var state = new TelloState(stateString);

                var observation = new AttitudeObservation(group, state);
                Assert.AreEqual(1, repo.Insert(observation));

                observation = repo.Read<AttitudeObservation>(1);
                Assert.IsNotNull(observation);
                Assert.AreEqual(1, observation.Id);
                Assert.AreEqual(state.Timestamp, observation.Timestamp);
                Assert.AreEqual(1, observation.GroupId);
                Assert.AreEqual(1, observation.Pitch);
                Assert.AreEqual(2, observation.Roll);
                Assert.AreEqual(3, observation.Yaw);
            }
        }

        [TestMethod]
        public void Battery_Insert_Read()
        {
            using (var repo = this.CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<Session>());
                Assert.IsTrue(repo.CreateCatalog<ObservationGroup>());
                Assert.IsTrue(repo.CreateCatalog<BatteryObservation>());

                var start = DateTime.UtcNow;
                var session = repo.NewEntity<Session>(start, TimeSpan.FromSeconds(1));
                Assert.IsNotNull(session);
                Assert.AreEqual(1, session.Id);
                Assert.AreEqual(start, session.StartTime);
                Assert.AreEqual(1000, session.Duration.TotalMilliseconds);

                var group = repo.NewEntity<ObservationGroup>(session, start);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);

                group = repo.Read<ObservationGroup>(1);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);

                var stateString = "mid:-1;x:0;y:0;z:0;mpry:0,0,0;pitch:1;roll:2;yaw:3;vgx:4;vgy:5;vgz:6;templ:7;temph:8;tof:9;h:10;bat:11;baro:12.13;time:14;agx:15.16;agy:17.18;agz:19.20;";
                var state = new TelloState(stateString);

                var observation = new BatteryObservation(group, state);
                Assert.AreEqual(1, repo.Insert(observation));

                observation = repo.Read<BatteryObservation>(1);
                Assert.IsNotNull(observation);
                Assert.AreEqual(1, observation.Id);
                Assert.AreEqual(state.Timestamp, observation.Timestamp);
                Assert.AreEqual(1, observation.GroupId);
                Assert.AreEqual(11, observation.PercentRemaining);
                Assert.AreEqual(8, observation.TemperatureHighC);
                Assert.AreEqual(7, observation.TemperatureLowC);
            }
        }

        [TestMethod]
        public void HobbsMeter_Insert_Read()
        {
            using (var repo = this.CreateRepository())
            {
                Assert.IsTrue(repo.CreateCatalog<Session>());
                Assert.IsTrue(repo.CreateCatalog<ObservationGroup>());
                Assert.IsTrue(repo.CreateCatalog<HobbsMeterObservation>());

                var start = DateTime.UtcNow;
                var session = repo.NewEntity<Session>(start, TimeSpan.FromSeconds(1));
                Assert.IsNotNull(session);
                Assert.AreEqual(1, session.Id);
                Assert.AreEqual(start, session.StartTime);
                Assert.AreEqual(1000, session.Duration.TotalMilliseconds);

                var group = repo.NewEntity<ObservationGroup>(session, start);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);

                group = repo.Read<ObservationGroup>(1);
                Assert.IsNotNull(group);
                Assert.AreEqual(1, group.Id);
                Assert.AreEqual(start, group.Timestamp);

                var stateString = "mid:-1;x:0;y:0;z:0;mpry:0,0,0;pitch:1;roll:2;yaw:3;vgx:4;vgy:5;vgz:6;templ:7;temph:8;tof:9;h:10;bat:11;baro:12.13;time:14;agx:15.16;agy:17.18;agz:19.20;";
                var state = new TelloState(stateString);

                var observation = new HobbsMeterObservation(group, state);
                Assert.AreEqual(1, repo.Insert(observation));

                observation = repo.Read<HobbsMeterObservation>(1);
                Assert.IsNotNull(observation);
                Assert.AreEqual(1, observation.Id);
                Assert.AreEqual(state.Timestamp, observation.Timestamp);
                Assert.AreEqual(1, observation.GroupId);
                Assert.AreEqual(9, observation.DistanceTraversedInCm);
                Assert.AreEqual(14, observation.MotorTimeInSeconds);
            }
        }
    }
}
