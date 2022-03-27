using System.Collections.Generic;
using System.Net;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Shouldly;
using Template.API.Infrastructure.Exceptions;
using Template.API.Infrastructure.Exceptions.Identity;
using Xunit;

namespace Template.Tests.API.Exceptions;

public class HttpResponseExceptionTests
{
    [Fact]
    public void ExpiredTokenException_Return_Correct_Response()
    {
        var exception = new ExpiredTokenException("token expired");
        var response = exception.CreateResponse();

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        response.ResultMessage.ShouldBe("Your login has expired. Please sign in again.");
    }

    [Fact]
    public void IncorrectPasswordException_Return_Correct_Response()
    {
        var message = "The provided password was incorrect.";
        var exception = new IncorrectPasswordException(message);
        var response = exception.CreateResponse();

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        response.ResultMessage.ShouldBe(message);
    }

    [Fact]
    public void NotFoundException_Return_Correct_Response()
    {
        var exception = new NotFoundException("EntityName", "0001");
        var response = exception.CreateResponse();

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        response.ResultMessage.ShouldBe($"Could not find EntityName with Id 0001");

        var intException = new NotFoundException("EntityName", 1);
        var intResponse = intException.CreateResponse();

        intResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        intResponse.ResultMessage.ShouldBe($"Could not find EntityName with Id 1");
    }

    [Fact]
    public void UserRegisterException_Return_Correct_Response()
    {
        var exception = new UserRegisterException(new List<IdentityError>());
        var response = exception.CreateResponse();

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.ResultMessage.ShouldBe(JsonConvert.SerializeObject(new List<IdentityError>()));
    }

    [Fact]
    public void ValidationException_Return_Correct_Response()
    {
        var exception = new ValidationException(new List<ValidationFailure>());
        var response = exception.CreateResponse();

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.ResultMessage.ShouldBe(JsonConvert.SerializeObject(new Dictionary<string, string[]>()));
    }
}