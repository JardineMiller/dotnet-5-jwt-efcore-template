using System.Threading.Tasks;
using Template.API.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Template.API.Infrastructure.Middleware
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly IExceptionLogger _logger;
        private readonly RequestDelegate _next;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, IExceptionLogger logger)
        {
            this._next = next;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this._next(context);
            }
            catch (HandledHttpResponseException ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, HandledHttpResponseException exception)
        {
            var response = exception.CreateResponse();
            this._logger.LogException(exception);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) response.StatusCode;

            var result = response.ResultMessage == string.Empty
                ? JsonConvert.SerializeObject(new {error = exception.Message})
                : response.ResultMessage;

            return context.Response.WriteAsync(result);
        }
    }
}
