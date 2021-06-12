using System;

namespace DevTracker.API.Infrastructure.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, int id) : base($"Could not find {name} with ID {id}") { }

        public NotFoundException(string name, string id) : base($"Could not find {name} {id}") { }
    }
}
