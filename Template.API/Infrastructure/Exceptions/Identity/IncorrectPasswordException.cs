using System.Net;
using Template.API.Infrastructure.Exceptions.Models;

namespace Template.API.Infrastructure.Exceptions.Identity
{
    public class IncorrectPasswordException : HandledHttpResponseException
    {
        public IncorrectPasswordException(string message) : base(message) {}
        public override ExceptionHttpResponse CreateResponse()
        {
            return new ExceptionHttpResponse()
            {
                StatusCode = HttpStatusCode.Unauthorized,
                ResultMessage = this.Message
            };
        }
    }
}
