using System;
using System.Threading.Tasks;

namespace Tello.Emulator.SDKV2
{
    public sealed class Position
    {
        public int X { get; internal set; } = 0;
        public int Y { get; internal set; } = 0;
        public int Z { get; internal set; } = 0;
        public int Heading { get; internal set; } = 0;
    }

    public sealed class FlightController
    {
        private StateServer _stateServer;
        private DroneState _droneState;
        private VideoServer _videoServer = new VideoServer();

        public bool IsVideoOn { get; private set; } = false;
        public int Speed { get; private set; } = 10;
        public Position Position { get; } = new Position();

        public enum FlightStates
        {
            StandingBy,
            Takingoff,
            InFlight,
            Landing,
            EmergencyStop
        }

        public bool IsSdkModeActivated { get; private set; } = false;
        public FlightStates FlightState { get; private set; } = FlightStates.StandingBy;

        public Task EnterSdkMode()
        {
            return Task.Run(async () =>
            {
                if (!IsSdkModeActivated)
                {
                    IsSdkModeActivated = true;
                    _droneState = new DroneState();
                    _stateServer = new StateServer(_droneState);
                    _stateServer.Start();
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });
        }

        public Task TakeOff()
        {
            return Task.Run(async () =>
            {
                if (IsSdkModeActivated && FlightState == FlightStates.StandingBy)
                {
                    _droneState.MotorClock.Start();
                    FlightState = FlightStates.Takingoff;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    _droneState.HeightInCm = 20;
                    FlightState = FlightStates.InFlight;
                }
            });
        }

        public Task Land()
        {
            return Task.Run(async () =>
            {
                if (IsSdkModeActivated &&
                    (FlightState == FlightStates.InFlight
                    || FlightState == FlightStates.Takingoff))
                {
                    FlightState = FlightStates.Landing;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    _droneState.MotorClock.Stop();
                    _droneState.HeightInCm = 0;
                    FlightState = FlightStates.StandingBy;
                }
            });
        }

        public Task EmergencyStop()
        {
            return Task.Run(async () =>
            {
                if (IsSdkModeActivated && FlightState != FlightStates.StandingBy)
                {
                    FlightState = FlightStates.EmergencyStop;
                    _droneState.MotorClock.Stop();
                    _droneState.HeightInCm = 0;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });
        }

        public Task SetSpeed(int speed)
        {
            if (speed < 10 || speed > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(speed));
            }

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                Speed = speed;
            });
        }

        //todo: set an approximate acceleration in the movement commands before the delay and reset to zero after
        public Task GoForward(int cm)
        {
            if (cm < 20 || cm > 500)
            {
                throw new ArgumentOutOfRangeException(nameof(cm));
            }

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(cm / Speed));
                Position.X += cm;
            });
        }

        public Task GoBack(int cm)
        {
            if (cm < 20 || cm > 500)
            {
                throw new ArgumentOutOfRangeException(nameof(cm));
            }

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(cm / Speed));
                Position.X -= cm;
            });
        }

        public Task GoRight(int cm)
        {
            if (cm < 20 || cm > 500)
            {
                throw new ArgumentOutOfRangeException(nameof(cm));
            }

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(cm / Speed));
                Position.Y += cm;
            });
        }

        public Task GoLeft(int cm)
        {
            if (cm < 20 || cm > 500)
            {
                throw new ArgumentOutOfRangeException(nameof(cm));
            }

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(cm / Speed));
                Position.Y -= cm;
            });
        }

        public Task GoUp(int cm)
        {
            if (cm < 20 || cm > 500)
            {
                throw new ArgumentOutOfRangeException(nameof(cm));
            }

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(cm / Speed));
                Position.Z += cm;
            });
        }

        public Task GoDown(int cm)
        {
            if (cm < 20 || cm > 500)
            {
                throw new ArgumentOutOfRangeException(nameof(cm));
            }

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(cm / Speed));
                Position.Z -= cm;
                if (Position.Z < 0)
                {
                    Position.Z = 0;
                }
            });
        }

        public Task TurnClockwise(int degrees)
        {
            if (degrees < 1 || degrees > 360)
            {
                throw new ArgumentOutOfRangeException(nameof(degrees));
            }

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                Position.Heading += degrees;
                if (Position.Heading > 360)
                {
                    Position.Heading -= 360;
                }
            });
        }

        public Task TurnCounterClockwise(int degrees)
        {
            if (degrees < 1 || degrees > 360)
            {
                throw new ArgumentOutOfRangeException(nameof(degrees));
            }

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                Position.Heading -= degrees;
                if (Position.Heading < 0)
                {
                    Position.Heading += 360;
                }
            });
        }

        public Task Go(int x, int y, int z, int speed)
        {
            if (x < -500 || x > 500)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }
            if (y < -500 || y > 500)
            {
                throw new ArgumentOutOfRangeException(nameof(y));
            }
            if (z < -500 || z > 500)
            {
                throw new ArgumentOutOfRangeException(nameof(z));
            }
            if (speed < 10 || speed > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(speed));
            }

            return Task.Run(async () =>
            {
                var distance = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                await Task.Delay(TimeSpan.FromSeconds(distance / Speed));
                Position.X += x;
                Position.Y += y;
                Position.Z += z;
                if (Position.Z < 0)
                {
                    Position.Z = 0;
                }
            });
        }

        public void StartVideo()
        {
            if (!IsVideoOn)
            {
                IsVideoOn = true;
                _videoServer.Start();
            }
        }

        public void StopVideo()
        {
            if (IsVideoOn)
            {
                IsVideoOn = false;
                _videoServer.Stop();
            }
        }

        public int GetSpeed()
        {
            return Speed;
        }

        public int GetBattery()
        {
            return _droneState.BatteryPercent;
        }

        public int GetTime()
        {
            return _droneState.MotorTimeInSeconds;
        }

    }
}
