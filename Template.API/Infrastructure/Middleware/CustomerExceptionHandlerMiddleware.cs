using System;
using System.Net;
using System.Threading.Tasks;
using Template.API.Infrastructure.Exceptions;
using Template.API.Infrastructure.Exceptions.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Template.API.Infrastructure.Middleware
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly ILogger<CustomExceptionHandlerMiddleware> logger;
        private readonly RequestDelegate next;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlerMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            switch (exception)
            {
                // case UserAccessViolation userAccessViolation:
                //     this.logger.LogWarning($"[{nameof(UserAccessViolation)}] occurred: [{userAccessViolation.Message}]");
                //     code = HttpStatusCode.Unauthorized;
                //     result = "You are not permitted to perform this action.";
                //     break;

                case ExpiredTokenException expiredTokenException:
                    code = HttpStatusCode.Unauthorized;
                    result = "Your login has expired. Please sign in again.";
                    break;

                case NotFoundException notFoundException:
                    this.logger.LogWarning($"[{nameof(NotFoundException)}] occurred: [{notFoundException.Message}]");
                    code = HttpStatusCode.NotFound;
                    result = notFoundException.Message;
                    break;

                case ValidationException validationException:
                    this.logger.LogWarning($"[{nameof(ValidationException)}] occurred: [{JsonConvert.SerializeObject(validationException.Failures)}]");
                    code = HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(validationException.Failures);
                    break;

                case IncorrectPasswordException incorrectPasswordException:
                    code = HttpStatusCode.Unauthorized;
                    result = incorrectPasswordException.Message;
                    break;

                case UserRegisterException userRegisterException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(userRegisterException.Errors);
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) code;

            result = result == string.Empty
                ? JsonConvert.SerializeObject(new {error = exception.Message})
                : result;

            return context.Response.WriteAsync(result);
        }
    }
}
