using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Template.API.Features.Identity.Factories;
using Template.API.Features.Identity.Models;
using Template.API.Infrastructure.Exceptions;
using Template.API.Infrastructure.Exceptions.Identity;
using Template.DAL;
using Template.DAL.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Template.API.Features.Identity.Commands
{
    public class RefreshTokenCommand : IRequest<LoginResponseModel>
    {
        public string Token { get; set; }
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponseModel>
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenFactory _tokenFactory;

        public RefreshTokenCommandHandler(ApplicationDbContext context, TokenFactory tokenFactory)
        {
            this._context = context;
            this._tokenFactory = tokenFactory;
        }

        public async Task<LoginResponseModel> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = this._context
                .Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefault(u => u.RefreshTokens.Any(t => t.Token == request.Token));

            if (user == null)
            {
                throw new NotFoundException(nameof(User), $"with token {request.Token}");
            }

            var oldRefreshToken = user.RefreshTokens
                .Single(x => x.Token == request.Token);

            if (!oldRefreshToken.IsActive)
            {
                throw new ExpiredTokenException("Unable to refresh token as it is expired");
            }

            // replace old refresh token with a new one and save
            var newRefreshToken = this._tokenFactory.GenerateRefreshToken();

            oldRefreshToken.RevokedOn = DateTime.UtcNow;
            oldRefreshToken.ReplacedBy = newRefreshToken.Token;

            user.RefreshTokens.Add(newRefreshToken);
            this._context.Update(user);
            var task = this._context.SaveChangesAsync(cancellationToken);

            // generate new jwt
            var jwt = this._tokenFactory.GenerateJwtToken(user.Id, user.UserName);

            var response = new LoginResponseModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Token = jwt,
                RefreshToken = newRefreshToken.Token
            };

            await task;
            return response;
        }
    }
}
