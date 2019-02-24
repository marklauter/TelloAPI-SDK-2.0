using System;
using Tello.Messaging;

namespace Tello.Controller
{
    public class RefinedDroneState : IRefinedDroneState
    {
        public RefinedDroneState() { }

        public RefinedDroneState(IPosition position, IAttitude attitude, IAirSpeed airSpeed, IBattery battery, IHobbsMeter hobbsMeter) {
            Position = position ?? throw new ArgumentNullException(nameof(position));
            Attitude = attitude ?? throw new ArgumentNullException(nameof(attitude));
            AirSpeed = airSpeed ?? throw new ArgumentNullException(nameof(airSpeed));
            Battery = battery ?? throw new ArgumentNullException(nameof(battery));
            HobbsMeter = hobbsMeter ?? throw new ArgumentNullException(nameof(hobbsMeter));
        }

        public IPosition Position { get; }
        public IAttitude Attitude { get; }
        public IAirSpeed AirSpeed { get; }
        public IBattery Battery { get; }
        public IHobbsMeter HobbsMeter { get; }
    }
}
