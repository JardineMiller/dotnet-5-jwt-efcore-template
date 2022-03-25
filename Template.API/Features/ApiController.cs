using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Template.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ApiController : ControllerBase
    {
        private IMediator mediator;

        protected IMediator Mediator => this.mediator ??= this.HttpContext.RequestServices.GetService<IMediator>();
    }
}