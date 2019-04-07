using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Tello.Controller;
using Tello.Messaging;

namespace Tello.Repository.Test
{
    public class Position : IPosition
    {
        public int AltitudeAGLInCm { get; set; }
        public double AltitudeMSLInCm { get; set; }
        public int Heading { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class Attitude : IAttitude
    {
        public int Pitch { get; set; }
        public int Roll { get; set; }
        public int Yaw { get; set; }
    }

    public class AirSpeed : IAirSpeed
    {
        public int SpeedX { get; set; }
        public int SpeedY { get; set; }
        public int SpeedZ { get; set; }
        public double AccelerationX { get; set; }
        public double AccelerationY { get; set; }
        public double AccelerationZ { get; set; }
    }

    public class Battery : IBattery
    {
        public int TemperatureLowC { get; set; }
        public int TemperatureHighC { get; set; }
        public int PercentRemaining { get; set; }
    }

    public class HobbsMeter : IHobbsMeter
    {
        public int DistanceTraversedInCm { get; set; }
        public int MotorTimeInSeconds { get; set; }
        public double FlightTimeRemainingInMinutes => (15 * 60 - MotorTimeInSeconds) / 60.0;
    }

    public class TestObservation : Observation
    {
        public TestObservation() : base() { }
        public TestObservation(string name) : base(true)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    [TestClass]
    public class RepositoryTest
    {
        [TestMethod]
        public async Task WriteTestObservation()
        {
            var repo = new ObservationRepository($"{nameof(WriteTestObservation)}.sqlite");
            await repo.Clear<TestObservation>();

            var observation = new TestObservation("test");
            await repo.Write(observation);

            var results = await repo.Read<TestObservation>();
            Assert.AreEqual(1, results.Length);
        }

        [TestMethod]
        public async Task ReadTestObservation()
        {
            var repo = new ObservationRepository($"{nameof(ReadTestObservation)}.sqlite");
            await repo.Clear<TestObservation>();

            await repo.Write(new TestObservation("one"));
            await repo.Write(new TestObservation("two"));

            var results = await repo.Read<TestObservation>(o => o.Name == "one");
            Assert.AreEqual(1, results.Length);
        }

        [TestMethod]
        public async Task WriteTelloStateObservation()
        {
            var repo = new ObservationRepository($"{nameof(WriteTelloStateObservation)}.sqlite");

            var position = new Position() { Heading = 0, AltitudeAGLInCm = 1, AltitudeMSLInCm = 2, X = 3, Y = 4 };
            var attitude = new Attitude() { Pitch = 0, Roll = 1, Yaw = 2 };
            var airSpeed = new AirSpeed()
            {
                AccelerationX = 0,
                AccelerationY = 1,
                AccelerationZ = 2,
                SpeedX = 0,
                SpeedY = 1,
                SpeedZ = 2
            };
            var battery = new Battery() { PercentRemaining = 0, TemperatureLowC = 1, TemperatureHighC = 2 };
            var hobbsMeter = new HobbsMeter() { DistanceTraversedInCm = 0, MotorTimeInSeconds = 1 };

            var telloState = new TelloState(position, attitude, airSpeed, battery, hobbsMeter);
            var groupId = Guid.NewGuid().ToString();

            await repo.Clear<PositionObservation>();
            await repo.Clear<AttitudeObservation>();
            await repo.Clear<AirSpeedObservation>();
            await repo.Clear<BatteryObservation>();
            await repo.Clear<HobbsMeterObservation>();

            await repo.Write(new PositionObservation(telloState, groupId));
            await repo.Write(new AttitudeObservation(telloState, groupId));
            await repo.Write(new AirSpeedObservation(telloState, groupId));
            await repo.Write(new BatteryObservation(telloState, groupId));
            await repo.Write(new HobbsMeterObservation(telloState, groupId));

            Assert.AreEqual(1, (await repo.Read<PositionObservation>()).Length);
            Assert.AreEqual(1, (await repo.Read<AttitudeObservation>()).Length);
            Assert.AreEqual(1, (await repo.Read<AirSpeedObservation>()).Length);
            Assert.AreEqual(1, (await repo.Read<BatteryObservation>()).Length);
            Assert.AreEqual(1, (await repo.Read<HobbsMeterObservation>()).Length);
        }
    }
}
