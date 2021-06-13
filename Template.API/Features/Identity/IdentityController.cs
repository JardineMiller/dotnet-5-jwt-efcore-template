using System;
using System.Threading.Tasks;
using Template.API.Controllers;
using Template.API.Features.Identity.Commands;
using Template.API.Features.Identity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Template.API.Features.Identity
{
    public class IdentityController : ApiController
    {
        [HttpPost]
        [Route(nameof(Register))]
        public async Task<ActionResult> Register(RegisterUserCommand cmd)
        {
            await Mediator.Send(cmd);
            return Ok();
        }

        [HttpPost]
        [Route(nameof(Login))]
        public async Task<ActionResult<LoginResponseModel>> Login(LoginCommand cmd)
        {
            var response = await Mediator.Send(cmd);

            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<LoginResponseModel>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
            {
                return null;
            }

            var cmd = new RefreshTokenCommand {Token = refreshToken};
            var response = await Mediator.Send(cmd);

            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public async Task<ActionResult> RevokeToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var cmd = new RevokeTokenCommand {Token = refreshToken};

            Response.Cookies.Delete("refreshToken");

            await Mediator.Send(cmd);
            return Ok();
        }

        private void SetTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true
            };

            Response.Cookies.Delete("refreshToken");
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
