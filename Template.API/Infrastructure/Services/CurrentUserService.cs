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
        private readonly ClaimsPrincipal _user;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this._user = httpContextAccessor.HttpContext?.User;
        }

        public bool IsAuthenticated() => this._user?.Identity?.IsAuthenticated ?? false;

        public string GetUserName() => this._user?.Identity?.Name;

        public string GetId() => this._user?.GetUsername();
    }
}
