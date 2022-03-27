using Template.API.Features.Tasks.Queries;
using Xunit;
using FluentValidation.TestHelper;

namespace Template.Tests.API.Tasks.Validators;

public class GetUserTasksQueryValidatorTests
{
    private readonly GetUserTasksQueryValidator _validator;

    public GetUserTasksQueryValidatorTests()
    {
        this._validator = new GetUserTasksQueryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Have_Error_When_UserId_Is_Invalid(string invalidUserId)
    {
        var query = new GetUserTasksQuery() { UserId = invalidUserId };
        var result = this._validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }
}