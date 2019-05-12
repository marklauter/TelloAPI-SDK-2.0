using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Tello;

namespace Messenger.Tello
{
    public class TelloMessenger : Messenger<string>
    {
        private readonly ConcurrentQueue<Command> _commands = new ConcurrentQueue<Command>();
        private readonly ITransceiver _transceiver;

        public TelloMessenger(ITransceiver transceiver)
        {
            _transceiver = transceiver ?? throw new ArgumentNullException(nameof(transceiver));
            ProcessCommandQueue();
        }

        private void Enqueue(Command command)
        {
            _commands.Enqueue(command);
        }

        public Task<TelloResponse> SendAsync(Commands command, params object[] args)
        {
            return SendAsync(new Command(command, args));
        }

        public async Task<TelloResponse> SendAsync(Command command)
        {
            if (command.Immediate)
            {
                return new TelloResponse(await _transceiver.SendAsync(new TelloRequest(command)));
            }
            else
            {
                Enqueue(command);
                return await Task.FromResult<TelloResponse>(null);
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
