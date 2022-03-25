using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Template.API.Infrastructure.Exceptions.Models;

namespace Template.API.Infrastructure.Exceptions.Identity
{
    public class UserRegisterException : HandledHttpResponseException
    {
        public UserRegisterException(IEnumerable<IdentityError> errors)
        {
            this.Errors = errors;
        }

        private IEnumerable<IdentityError> Errors { get; }
        
        public override ExceptionHttpResponse CreateResponse()
        {
            return new ExceptionHttpResponse()
            {
                StatusCode = HttpStatusCode.BadRequest,
                ResultMessage = JsonConvert.SerializeObject(this.Errors)
            };
        }
    }
}
