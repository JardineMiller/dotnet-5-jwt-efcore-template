using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Template.API.Infrastructure.Exceptions.Models;

namespace Template.API.Infrastructure.Exceptions.Identity
{
    public class UserRegisterException : HandledHttpResponseException
    {
        private readonly IEnumerable<IdentityError> _errors;

        public UserRegisterException(IEnumerable<IdentityError> errors)
        {
            this._errors = errors;
        }

        public override ExceptionHttpResponse CreateResponse()
        {
            return new ExceptionHttpResponse()
            {
                StatusCode = HttpStatusCode.BadRequest,
                ResultMessage = JsonConvert.SerializeObject(this._errors)
            };
        }
    }
}