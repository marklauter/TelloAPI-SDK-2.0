using System;

namespace Tello.Scripting
{
    public class TelloScriptException : Exception
    {
        public TelloScriptException()
        {
        }

        public TelloScriptException(string message) : base(message)
        {
        }

        public TelloScriptException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class BadTokenException : TelloScriptException
    {
        public BadTokenException()
        {
        }

        public BadTokenException(string message) : base(message)
        {
        }

        public BadTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class InvalidCommandException : BadTokenException
    {
        public InvalidCommandException()
        {
        }

        public InvalidCommandException(string message) : base(message)
        {
        }

        public InvalidCommandException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class TokenAlreadyExistsException : TelloScriptException
    {
        public TokenAlreadyExistsException()
        {
        }

        public TokenAlreadyExistsException(string message) : base(message)
        {
        }

        public TokenAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class TokenNotFoundException : TelloScriptException
    {
        public TokenNotFoundException()
        {
        }

        public TokenNotFoundException(string message) : base(message)
        {
        }

        public TokenNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
