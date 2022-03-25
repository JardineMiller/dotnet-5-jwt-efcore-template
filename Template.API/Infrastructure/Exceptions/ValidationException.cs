using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentValidation.Results;
using Newtonsoft.Json;
using Template.API.Infrastructure.Exceptions.Models;

namespace Template.API.Infrastructure.Exceptions
{
    public class ValidationException : HandledHttpResponseException
    {
        public ValidationException(): base("One or more validation failures have occurred.")
        {
            this.Failures = new Dictionary<string, string[]>();
        }

        public ValidationException(List<ValidationFailure> failures): this()
        {
            var propertyNames = failures
                .Select(e => e.PropertyName)
                .Distinct();

            foreach (var propertyName in propertyNames)
            {
                var propertyFailures = failures
                    .Where(e => e.PropertyName == propertyName)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                this.Failures.Add(propertyName, propertyFailures);
            }
        }

        public IDictionary<string, string[]> Failures { get; }
        
        public override ExceptionHttpResponse CreateResponse()
        {
            return new ExceptionHttpResponse()
            {
                StatusCode = HttpStatusCode.BadRequest,
                ResultMessage = JsonConvert.SerializeObject(this.Failures)
            };
        }
    }
}
