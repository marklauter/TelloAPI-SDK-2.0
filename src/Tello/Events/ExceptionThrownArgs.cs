using System;

namespace Tello.Events
{
    public sealed class ExceptionThrownArgs : EventArgs
    {
        public ExceptionThrownArgs(TelloException exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public TelloException Exception { get; }
    }
}
