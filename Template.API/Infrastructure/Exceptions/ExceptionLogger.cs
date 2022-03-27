using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Template.API.Infrastructure.Exceptions
{
    public class ExceptionLogger : IExceptionLogger
    {
        private readonly ILogger<ExceptionLogger> _logger;
        
        public ExceptionLogger(ILogger<ExceptionLogger> logger)
        {
            this._logger = logger;
        }
        
        public void LogException(ILoggableException exception)
        {
            switch (exception)
            {
                // case UserAccessViolation userAccessViolation:
                //     this.logger.LogWarning($"[{nameof(UserAccessViolation)}] occurred: [{userAccessViolation.Message}]");
                //     break;

                case NotFoundException notFoundException:
                    this._logger.LogWarning($"[{nameof(NotFoundException)}] occurred: [{notFoundException.Message}]");
                    break;

                case ValidationException validationException:
                    this._logger.LogWarning($"[{nameof(ValidationException)}] occurred: [{JsonConvert.SerializeObject(validationException.Failures)}]");
                    break;
            }
        }
    }

    public interface IExceptionLogger
    {
        void LogException(ILoggableException loggable);
    }
}