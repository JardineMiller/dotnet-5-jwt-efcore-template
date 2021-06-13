using System.Security.Claims;
using Template.API.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;

namespace Template.API.Infrastructure.Services
{
    public interface ICurrentUserService
    {

    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly ClaimsPrincipal user;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this.user = httpContextAccessor.HttpContext?.User;
        }

        public bool IsAuthenticated() => this.user?.Identity?.IsAuthenticated ?? false;

        public string GetUserName() => this.user?.Identity?.Name;

        public string GetId() => this.user?.GetUsername();
    }
}
