using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Template.API.Features.Tasks.Models;
using Template.API.Features.Tasks.Queries;
using Template.DAL;
using Template.Tests.Common;
using Xunit;

namespace Template.Tests.API.Tasks.Queries;

public class GetUserTasksTests
{
    private readonly ApplicationDbContext _context;

    public GetUserTasksTests()
    {
        var fixture = new QueryTestBase();
        this._context = fixture.Context;

        this._context.Tasks.Add(new DAL.Models.Entities.Task()
        {
            Name = "Task 1",
            Description = null,
            AssigneeId = "0001"
        });

        this._context.Tasks.Add(new DAL.Models.Entities.Task()
        {
            Name = "Task 2",
            Description = null,
            AssigneeId = "0001"
        });

        this._context.SaveChanges();
    }

    [Fact]
    public async Task GetUserTasks_WithNonExistentUserId_ShouldReturnNoTasks()
    {
        var sut = new GetUserTasksQueryHandler(this._context);

        // Arrange
        var nonExistingUserId = "INVALID";

        // Act
        var result = await sut.Handle(new GetUserTasksQuery() { UserId = nonExistingUserId }, CancellationToken.None);

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetUserTasks_WithExistingUserId_ShouldReturnTaskResponseModels()
    {
        var sut = new GetUserTasksQueryHandler(this._context);

        // Arrange
        var existingUserId = "0001";

        // Act
        var result = (await sut.Handle(new GetUserTasksQuery() { UserId = existingUserId }, CancellationToken.None))
            .ToList();

        // Assert
        result.ShouldBeOfType<List<TaskResponseModel>>();
        result.Count().ShouldBe(2);

        result.Count(x => x.Name == "Task 1").ShouldBe(1);
        result.Count(x => x.Name == "Task 2").ShouldBe(1);
        result.Count(x => x.Description == null).ShouldBe(2);
        result.Count(x => x.AssigneeId == "0001").ShouldBe(2);
    }
}