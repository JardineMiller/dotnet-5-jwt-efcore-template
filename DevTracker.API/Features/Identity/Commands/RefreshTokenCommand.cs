using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevTracker.API.Features.Identity.Factories;
using DevTracker.API.Features.Identity.Models;
using DevTracker.API.Infrastructure.Exceptions;
using DevTracker.API.Infrastructure.Exceptions.Identity;
using DevTracker.DAL;
using DevTracker.DAL.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevTracker.API.Features.Identity.Commands
{
    public class RefreshTokenCommand : IRequest<LoginResponseModel>
    {
        public string Token { get; set; }
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponseModel>
    {
        private readonly ApplicationDbContext context;
        private readonly TokenFactory tokenFactory;

        public RefreshTokenCommandHandler(ApplicationDbContext context, TokenFactory tokenFactory)
        {
            this.context = context;
            this.tokenFactory = tokenFactory;
        }

        public async Task<LoginResponseModel> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = context
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
            var newRefreshToken = tokenFactory.GenerateRefreshToken();

            oldRefreshToken.RevokedOn = DateTime.UtcNow;
            oldRefreshToken.ReplacedBy = newRefreshToken.Token;

            user.RefreshTokens.Add(newRefreshToken);
            context.Update(user);
            var task = context.SaveChangesAsync(cancellationToken);

            // generate new jwt
            var jwt = tokenFactory.GenerateJwtToken(user.Id, user.UserName);

            var response = new LoginResponseModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Token = jwt,
                RefreshToken = newRefreshToken.Token
            };

            await task;
            return response;
        }
    }
}
