using Messenger;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Tello.Messaging
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
                Debug.WriteLine($"{nameof(SendAsync)}: '{command}' command queue is {_commands.Count} deep.");
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
                        if (!_commands.IsEmpty && _commands.TryDequeue(out var command))
                        {
                            Debug.WriteLine($"{nameof(ProcessCommandQueue)}: command queue is {_commands.Count} deep.");

                            var request = new TelloRequest(command);
                            Debug.WriteLine($"{nameof(ProcessCommandQueue)}: request.Message '{request.Message}'");
                            Debug.WriteLine($"{nameof(ProcessCommandQueue)}: request.Timeout '{request.Timeout}'");

                            var response = new TelloResponse(await _transceiver.SendAsync(request));
                            Debug.WriteLine($"{nameof(ProcessCommandQueue)}: response.Success '{response.Success}'");
                            Debug.WriteLine($"{nameof(ProcessCommandQueue)}: response.Message '{response.Message}'");

                            ReponseReceived(response);
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
