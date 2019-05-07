using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using Repository.Sqlite;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Tello.State;

namespace Tello.Observations.Sqlite.Test
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
            using(var repo= CreateRepository())
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
            using (var repo = CreateRepository())
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
            using (var repo = CreateRepository())
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

                var args = new CommandResponseReceivedArgs(
                    Commands.Takeoff, 
                    "ok", 
                    start, 
                    TimeSpan.FromSeconds(1));
                var observation = new ResponseObservation(group, args);
                Assert.AreEqual(1, repo.Insert(observation));

                observation = repo.Read<ResponseObservation>(1);
                Assert.IsNotNull(observation);
                Assert.AreEqual(1, observation.Id);
                Assert.AreEqual(1, observation.GroupId);
                Assert.AreEqual(start, observation.TimeInitiated);
                Assert.AreEqual(Commands.Takeoff, observation.Command);
                Assert.AreEqual("ok", observation.Response);
            }
        }

        [TestMethod]
        public void PositionObservation_Insert_Read()
        {
            using (var repo = CreateRepository())
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

                var state = new TelloState(
                    new TestPosition(),
                    new TestAttitude(),
                    new TestAirSpeed(),
                    new TestBattery(),
                    new TestHobbsMeter(),
                    start,
                    "data");

                var observation = new PositionObservation(group, state);
                Assert.AreEqual(1, repo.Insert(observation));

                observation = repo.Read<PositionObservation>(1);
                Assert.IsNotNull(observation);
                Assert.AreEqual(1, observation.Id);
                Assert.AreEqual(start, observation.Timestamp);
                Assert.AreEqual(1, observation.GroupId);
                Assert.AreEqual(1, observation.AltitudeAGLInCm);
                Assert.AreEqual(1, observation.AltitudeMSLInCm);
                Assert.AreEqual(1, observation.Heading);
                Assert.AreEqual(1, observation.X);
                Assert.AreEqual(1, observation.Y);
            }
        }

        [TestMethod]
        public void AirSpeed_Insert_Read()
        {
            using (var repo = CreateRepository())
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

                var state = new TelloState(
                    new TestPosition(),
                    new TestAttitude(),
                    new TestAirSpeed(),
                    new TestBattery(),
                    new TestHobbsMeter(),
                    start,
                    "data");

                var observation = new AirSpeedObservation(group, state);
                Assert.AreEqual(1, repo.Insert(observation));

                observation = repo.Read<AirSpeedObservation>(1);
                Assert.IsNotNull(observation);
                Assert.AreEqual(1, observation.Id);
                Assert.AreEqual(start, observation.Timestamp);
                Assert.AreEqual(1, observation.GroupId);
                Assert.AreEqual(1, observation.AccelerationX);
                Assert.AreEqual(1, observation.AccelerationY);
                Assert.AreEqual(1, observation.AccelerationZ);
                Assert.AreEqual(1, observation.SpeedX);
                Assert.AreEqual(1, observation.SpeedY);
                Assert.AreEqual(1, observation.SpeedZ);
            }
        }

        [TestMethod]
        public void Attitude_Insert_Read()
        {
            using (var repo = CreateRepository())
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

                var state = new TelloState(
                    new TestPosition(),
                    new TestAttitude(),
                    new TestAirSpeed(),
                    new TestBattery(),
                    new TestHobbsMeter(),
                    start,
                    "data");

                var observation = new AttitudeObservation(group, state);
                Assert.AreEqual(1, repo.Insert(observation));

                observation = repo.Read<AttitudeObservation>(1);
                Assert.IsNotNull(observation);
                Assert.AreEqual(1, observation.Id);
                Assert.AreEqual(start, observation.Timestamp);
                Assert.AreEqual(1, observation.GroupId);
                Assert.AreEqual(1, observation.Pitch);
                Assert.AreEqual(1, observation.Roll);
                Assert.AreEqual(1, observation.Yaw);
            }
        }

        [TestMethod]
        public void Battery_Insert_Read()
        {
            using (var repo = CreateRepository())
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

                var state = new TelloState(
                    new TestPosition(),
                    new TestAttitude(),
                    new TestAirSpeed(),
                    new TestBattery(),
                    new TestHobbsMeter(),
                    start,
                    "data");

                var observation = new BatteryObservation(group, state);
                Assert.AreEqual(1, repo.Insert(observation));

                observation = repo.Read<BatteryObservation>(1);
                Assert.IsNotNull(observation);
                Assert.AreEqual(1, observation.Id);
                Assert.AreEqual(start, observation.Timestamp);
                Assert.AreEqual(1, observation.GroupId);
                Assert.AreEqual(1, observation.PercentRemaining);
                Assert.AreEqual(1, observation.TemperatureHighC);
                Assert.AreEqual(1, observation.TemperatureLowC);
            }
        }

        [TestMethod]
        public void HobbsMeter_Insert_Read()
        {
            using (var repo = CreateRepository())
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

                var state = new TelloState(
                    new TestPosition(),
                    new TestAttitude(),
                    new TestAirSpeed(),
                    new TestBattery(),
                    new TestHobbsMeter(),
                    start,
                    "data");

                var observation = new HobbsMeterObservation(group, state);
                Assert.AreEqual(1, repo.Insert(observation));

                observation = repo.Read<HobbsMeterObservation>(1);
                Assert.IsNotNull(observation);
                Assert.AreEqual(1, observation.Id);
                Assert.AreEqual(start, observation.Timestamp);
                Assert.AreEqual(1, observation.GroupId);
                Assert.AreEqual(1, observation.DistanceTraversedInCm);
                Assert.AreEqual(1, observation.MotorTimeInSeconds);
            }
        }

    }
}

