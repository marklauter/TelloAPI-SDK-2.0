using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Tello;

namespace Messenger.Tello
{
    public class TelloMessenger : Messenger, IDisposable
    {
        private readonly ConcurrentQueue<Command> _commands = new ConcurrentQueue<Command>();
        private readonly ITransceiver _transceiver;
        private readonly CancellationTokenSource _cancellationTokenSource = null;

        public TelloMessenger(ITransceiver transceiver)
        {
            _transceiver = transceiver ?? throw new ArgumentNullException(nameof(transceiver));
            ProcessCommandQueue();
        }

        private void Enqueue(Command command)
        {
            _commands.Enqueue(command);
        }

        public void Send(Commands command, params object[] args)
        {
            try
            {
                Send(new Command(command, args));
            }
            catch (Exception ex)
            {
                ExceptionThrown(ex);
            }
        }

        public async void Send(Command command)
        {
            try
            {
                if (!command.Immediate)
                {
                    Enqueue(command);
                }
                else
                {
                    var response = new TelloResponse(await _transceiver.SendAsync(new TelloRequest(command)));
                    ReponseReceived(response);
                }
            }
            catch (Exception ex)
            {
                ExceptionThrown(ex);
            }
        }

        private async void ProcessCommandQueue()
        {
            await Task.Run(async () =>
            {
                var spinWait = new SpinWait();
                while (true)
                {
                    try
                    {
                        if (!_commands.IsEmpty)
                        {
                            if (_commands.TryDequeue(out var command))
                            {
                                var response = new TelloResponse(await _transceiver.SendAsync(new TelloRequest(command)));
                                ReponseReceived(response);
                            }
                        }
                        else
                        {
                            spinWait.SpinOnce();
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionThrown(ex);
                    }
                }
            });
        }
    }
}
