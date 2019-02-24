using System;

namespace Tello.Controller
{
    public interface ITelloStateChangedNotifier
    {
        event EventHandler<TelloStateChangedArgs> DroneStateChanged;
    }
}
