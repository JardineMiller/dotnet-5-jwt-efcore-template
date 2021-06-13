using System.Linq;
using System.Security.Claims;

namespace Template.API.Infrastructure.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user
                .Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
