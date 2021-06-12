using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevTracker.API.Infrastructure.Exceptions;
using DevTracker.API.Infrastructure.Exceptions.Identity;
using DevTracker.DAL;
using DevTracker.DAL.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevTracker.API.Features.Identity.Commands
{
    public class RevokeTokenCommand : IRequest<bool>
    {
        public string Token { get; set; }
    }

    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, bool>
    {
        private readonly ApplicationDbContext context;

        public RevokeTokenCommandHandler(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            var user = context
                .Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefault(u => u.RefreshTokens.Any(t => t.Token == request.Token));

            // return false if no user found with token
            if (user == null)
            {
                throw new NotFoundException(nameof(User), $"with token {request.Token}");
            }

            var refreshToken = user.RefreshTokens
                .Single(x => x.Token == request.Token);

            // return false if token is not active
            if (!refreshToken.IsActive)
            {
                throw new ExpiredTokenException("Unable to revoke token as it is already expired");
            }

            // revoke token and save
            refreshToken.RevokedOn = DateTime.UtcNow;

            context.Update(user);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
