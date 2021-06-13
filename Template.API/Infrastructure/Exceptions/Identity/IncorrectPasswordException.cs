using System;

namespace Template.API.Infrastructure.Exceptions.Identity
{
    public class IncorrectPasswordException : Exception
    {
        public IncorrectPasswordException(string message) : base(message) {}
    }
}
