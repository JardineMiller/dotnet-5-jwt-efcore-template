using System;
using Template.API.Infrastructure.Exceptions.Models;

namespace Template.API.Infrastructure.Exceptions
{
    public abstract class HandledHttpResponseException : Exception, IHandledResponseException, ILoggableException
    {
        protected HandledHttpResponseException(string message) : base(message) {}
        protected HandledHttpResponseException() : base() {}

        public abstract ExceptionHttpResponse CreateResponse();
    }

    public interface ILoggableException
    {
    }

    public interface IHandledResponseException
    {
        ExceptionHttpResponse CreateResponse();
    }
}