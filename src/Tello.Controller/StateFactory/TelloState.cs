using System;
using Tello.Messaging;

namespace Tello.Controller
{
    public class TelloState : ITelloState
    {
        public TelloState() { }

        public TelloState(IPosition position, IAttitude attitude, IAirSpeed airSpeed, IBattery battery, IHobbsMeter hobbsMeter)
        {
            Timestamp = DateTime.UtcNow;
            Position = position ?? throw new ArgumentNullException(nameof(position));
            Attitude = attitude ?? throw new ArgumentNullException(nameof(attitude));
            AirSpeed = airSpeed ?? throw new ArgumentNullException(nameof(airSpeed));
            Battery = battery ?? throw new ArgumentNullException(nameof(battery));
            HobbsMeter = hobbsMeter ?? throw new ArgumentNullException(nameof(hobbsMeter));
        }

        public DateTime Timestamp { get; }
        public IPosition Position { get; }
        public IAttitude Attitude { get; }
        public IAirSpeed AirSpeed { get; }
        public IBattery Battery { get; }
        public IHobbsMeter HobbsMeter { get; }
    }
}
