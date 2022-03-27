using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Template.API.Features.Tasks.Models;
using Template.API.Features.Tasks.Queries;

namespace Template.API.Features.Tasks
{
    public class TasksController : ApiController
    {
        [HttpGet]
        [Route("{userId}")]
        public async Task<ActionResult<IEnumerable<TaskResponseModel>>> GetUserTasks(string userId)
        {
            var query = new GetUserTasksQuery() { UserId = userId };
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}