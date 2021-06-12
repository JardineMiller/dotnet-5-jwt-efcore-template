using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace DevTracker.API.Infrastructure.Exceptions.Identity
{
    public class UserRegisterException : Exception
    {
        public UserRegisterException(IEnumerable<IdentityError> errors)
        {
            this.Errors = errors;
        }

        public IEnumerable<IdentityError> Errors { get; }
    }
}
