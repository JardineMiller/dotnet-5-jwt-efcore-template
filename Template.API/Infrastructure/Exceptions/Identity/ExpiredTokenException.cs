using System.Net;
using Template.API.Infrastructure.Exceptions.Models;

namespace Template.API.Infrastructure.Exceptions.Identity
{
    public class ExpiredTokenException : HandledHttpResponseException
    {
        public ExpiredTokenException(string message) : base(message) { }

        public override ExceptionHttpResponse CreateResponse()
        {
            return new ExceptionHttpResponse()
            {
                StatusCode = HttpStatusCode.Unauthorized,
                ResultMessage = "Your login has expired. Please sign in again."
            };
        }
    }
}
