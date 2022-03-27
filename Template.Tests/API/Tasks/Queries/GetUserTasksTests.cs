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
        _context = fixture.Context;
    }

    [Fact]
    public async Task GetUserTasks_WithNonExistentUserId_ShouldReturnNoTasks()
    {
        // Arrange
        var sut = new GetUserTasksQueryHandler(this._context);
        var nonExistingUserId = "INVALID";

        // Act
        var result = await sut.Handle(new GetUserTasksQuery() {UserId = nonExistingUserId}, CancellationToken.None);

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetUserTasks_WithExistingUserId_ShouldReturnTaskResponseModels()
    {
        // Arrange
        _context.Tasks.Add(new DAL.Models.Entities.Task()
        {
            Name = "Task 1",
            Description = null,
            AssigneeId = "0001"
        });

        _context.Tasks.Add(new DAL.Models.Entities.Task()
        {
            Name = "Task 2",
            Description = null,
            AssigneeId = "0001"
        });

        _context.SaveChanges();

        var sut = new GetUserTasksQueryHandler(this._context);

        var existingUserId = "0001";

        // Act
        var result = await sut.Handle(new GetUserTasksQuery() {UserId = existingUserId}, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<List<TaskResponseModel>>();
        result.Count().ShouldBe(2);
    }
}