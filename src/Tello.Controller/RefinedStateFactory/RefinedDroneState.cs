using Tello.Messaging;

namespace Tello.Controller
{
    public class RefinedDroneState : IRefinedDroneState
    {
        public IPosition Position { get; set; }
        public IAttitude Attitude { get; set; }
        public IAirSpeed AirSpeed { get; set; }
        public IBattery Battery { get; set; }
        public IHobbsMeter HobbsMeter { get; set; }
    }
}
