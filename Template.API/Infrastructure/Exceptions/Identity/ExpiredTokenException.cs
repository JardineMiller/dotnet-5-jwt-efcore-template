using System;

namespace Template.API.Infrastructure.Exceptions.Identity
{
    public class ExpiredTokenException : Exception
    {
        public ExpiredTokenException(string message) : base(message) { }
    }
}
