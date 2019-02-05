using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Tello.Messaging;

namespace Tello.Emulator.SDKV2
{
    internal sealed class StateManager
    {
        public StateManager(DroneState droneState, VideoServer videoServer, StateServer stateServer)
        {
            _droneState = droneState ?? throw new ArgumentNullException(nameof(droneState));
            _videoServer = videoServer ?? throw new ArgumentNullException(nameof(videoServer));
            _stateServer = stateServer ?? throw new ArgumentNullException(nameof(stateServer));
            DischargeBattery();
        }

        //private readonly Gate _gate = new Gate();
        private readonly DroneState _droneState;
        private readonly VideoServer _videoServer;
        private readonly StateServer _stateServer;
        private readonly Position _position = new Position();
        private readonly Stopwatch _batteryClock = new Stopwatch();

        public bool IsPoweredUp { get; private set; } = false;
        public bool IsVideoOn { get; private set; } = false;
        public bool IsSdkModeActivated { get; private set; } = false;
        public int Speed { get; private set; } = 10;
        public Position Position => new Position(_position);
        public FlightStates FlightState { get; private set; } = FlightStates.StandingBy;

        public void RechargeBattery()
        {
            _droneState.BatteryPercent = 100;
        }

        public void PowerOn()
        {
            if (!IsPoweredUp)
            {
                IsPoweredUp = true;
                _batteryClock.Start();
                _stateServer.Start();
            }
        }

        public void PowerOff()
        {
            if (IsPoweredUp)
            {
                IsPoweredUp = false;
                _batteryClock.Stop();
                _stateServer.Stop();
                StopVideo();
            }
        }

        public enum FlightStates
        {
            StandingBy,
            Takingoff,
            InFlight,
            Landing,
            EmergencyStop
        }

        public Task EnterSdkMode()
        {
            if (!IsPoweredUp)
            {
                return Task.CompletedTask;
            }

            return Task.Run(async () =>
            {
                if (!IsSdkModeActivated)
                {
                    IsSdkModeActivated = true;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });
        }

        public Task TakeOff()
        {
            if (!IsPoweredUp)
            {
                return Task.CompletedTask;
            }

            return Task.Run(async () =>
            {
                if (IsSdkModeActivated && FlightState == FlightStates.StandingBy)
                {
                    _droneState.MotorClock.Start();
                    FlightState = FlightStates.Takingoff;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    _droneState.HeightInCm = 20;
                    _position.Z = _droneState.HeightInCm;
                    FlightState = FlightStates.InFlight;
                }
            });
        }

        public Task Land()
        {
            if (!IsPoweredUp)
            {
                return Task.CompletedTask;
            }

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
                    _position.Z = _droneState.HeightInCm;
                    FlightState = FlightStates.StandingBy;
                }
            });
        }

        public Task EmergencyStop()
        {
            if (!IsPoweredUp)
            {
                return Task.CompletedTask;
            }

            return Task.Run(async () =>
            {
                if (IsSdkModeActivated && FlightState != FlightStates.StandingBy)
                {
                    FlightState = FlightStates.EmergencyStop;
                    _droneState.MotorClock.Stop();
                    _droneState.HeightInCm = 0;
                    _position.Z = _droneState.HeightInCm;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });
        }

        public Task SetSpeed(int speed)
        {
            if (!IsPoweredUp)
            {
                return Task.CompletedTask;
            }

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
            if (!IsPoweredUp)
            {
                return Task.CompletedTask;
            }

            if (cm < 20 || cm > 500)
            {
                throw new ArgumentOutOfRangeException(nameof(cm));
            }

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(cm / Speed));
                _position.X += cm;
            });
        }

        public Task GoBack(int cm)
        {
            if (!IsPoweredUp)
            {
                return Task.CompletedTask;
            }

            if (cm < 20 || cm > 500)
            {
                throw new ArgumentOutOfRangeException(nameof(cm));
            }

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(cm / Speed));
                _position.X -= cm;
            });
        }

        public Task GoRight(int cm)
        {
            if (!IsPoweredUp)
            {
                return Task.CompletedTask;
            }

            if (cm < 20 || cm > 500)
            {
                throw new ArgumentOutOfRangeException(nameof(cm));
            }

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(cm / Speed));
                _position.Y += cm;
            });
        }

        public Task GoLeft(int cm)
        {
            if (!IsPoweredUp)
            {
                return Task.CompletedTask;
            }

            if (cm < 20 || cm > 500)
            {
                throw new ArgumentOutOfRangeException(nameof(cm));
            }

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(cm / Speed));
                _position.Y -= cm;
            });
        }

        public Task GoUp(int cm)
        {
            if (!IsPoweredUp)
            {
                return Task.CompletedTask;
            }

            if (cm < 20 || cm > 500)
            {
                throw new ArgumentOutOfRangeException(nameof(cm));
            }

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(cm / Speed));
                _droneState.HeightInCm += cm;
                _position.Z = _droneState.HeightInCm;
            });
        }

        public Task GoDown(int cm)
        {
            if (!IsPoweredUp)
            {
                return Task.CompletedTask;
            }

            if (cm < 20 || cm > 500)
            {
                throw new ArgumentOutOfRangeException(nameof(cm));
            }

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(cm / Speed));
                _droneState.HeightInCm -= cm;
                if (_droneState.HeightInCm < 0)
                {
                    _droneState.HeightInCm = 0;
                }
                _position.Z = _droneState.HeightInCm;
            });
        }

        public Task TurnClockwise(int degrees)
        {
            if (!IsPoweredUp)
            {
                return Task.CompletedTask;
            }

            if (degrees < 1 || degrees > 360)
            {
                throw new ArgumentOutOfRangeException(nameof(degrees));
            }

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                _position.Heading += degrees;
                if (_position.Heading > 360)
                {
                    _position.Heading -= 360;
                }
            });
        }

        public Task TurnCounterClockwise(int degrees)
        {
            if (!IsPoweredUp)
            {
                return Task.CompletedTask;
            }

            if (degrees < 1 || degrees > 360)
            {
                throw new ArgumentOutOfRangeException(nameof(degrees));
            }

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                _position.Heading -= degrees;
                if (_position.Heading < 0)
                {
                    _position.Heading += 360;
                }
            });
        }

        public Task Go(int x, int y, int z, int speed)
        {
            if (!IsPoweredUp)
            {
                return Task.CompletedTask;
            }

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
                _position.X += x;
                _position.Y += y;
                _position.Z += z;
                if (_position.Z < 0)
                {
                    _position.Z = 0;
                }
            });
        }

        public Task StartVideo()
        {
            if (!IsPoweredUp)
            {
                return Task.CompletedTask;
            }

            return Task.Run(async () =>
            {
                await Task.Delay(500);
                if (!IsVideoOn)
                {
                    IsVideoOn = true;
                    _videoServer.Start();
                }
            });
        }

        public Task StopVideo()
        {
            if (!IsPoweredUp)
            {
                return Task.CompletedTask;
            }

            return Task.Run(async () =>
            {
                await Task.Delay(500);
                if (IsVideoOn)
                {
                    IsVideoOn = false;
                    _videoServer.Stop();
                }
            });
        }

        public Task<int> GetSpeed()
        {
            if (!IsPoweredUp)
            {
                return Task.FromResult(-1);
            }

            return Task.Run(async () =>
            {
                await Task.Delay(500);
                return Speed;
            });
        }

        public Task<int> GetBattery()
        {
            if (!IsPoweredUp)
            {
                return Task.FromResult(-1);
            }

            return Task.Run(async () =>
            {
                await Task.Delay(500);
                return _droneState.BatteryPercent;
            });
        }

        public Task<int> GetTime()
        {
            if (!IsPoweredUp)
            {
                return Task.FromResult(-1);
            }

            return Task.Run(async () =>
            {
                await Task.Delay(500);
                return _droneState.MotorTimeInSeconds;
            });
        }

        private async void DischargeBattery()
        {
            // documentation says there's ~ 15 minutes of battery
            await Task.Run(() =>
            {
                var wait = new SpinWait();
                while (true)
                {
                    if (IsPoweredUp)
                    {
                        _droneState.BatteryPercent = 100 - (int)(_batteryClock.Elapsed.TotalMinutes / 15.0 * 100);

                        if (_droneState.BatteryPercent < 1)
                        {
                            _droneState.BatteryPercent = 0;
                            PowerOff();
                            Log.WriteLine("battery died");
                        }
                    }
                    wait.SpinOnce();
                }
            });
        }
    }
}
