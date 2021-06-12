using System;

namespace DevTracker.API.Infrastructure.Exceptions.Identity
{
    public class IncorrectPasswordException : Exception
    {
        public IncorrectPasswordException(string message) : base(message) {}
    }
}
