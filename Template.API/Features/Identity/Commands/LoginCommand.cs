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
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Template.API.Features.Identity.Commands
{
    public class LoginCommand : IRequest<LoginResponseModel>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseModel>
    {
        private readonly ApplicationDbContext context;
        private readonly TokenFactory tokenFactory;
        private readonly UserManager<User> userManager;

        public LoginCommandHandler(UserManager<User> userManager, TokenFactory tokenFactory,
            ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.tokenFactory = tokenFactory;
            this.context = context;
        }

        public async Task<LoginResponseModel> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.Users
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.UserName == request.Username, cancellationToken);

            await ValidateUserInfo(request, user);

            var jwt = tokenFactory.GenerateJwtToken(user.Id, user.UserName);
            var refreshToken = tokenFactory.GenerateRefreshToken();

            if (user.RefreshTokens.Any())
            {
                var lastToken = user.RefreshTokens.Last();

                if (lastToken.IsActive)
                {
                    lastToken.ReplacedBy = refreshToken.Token;
                    lastToken.RevokedOn = DateTime.UtcNow;
                }
            }

            user.RefreshTokens.Add(refreshToken);

            context.Update(user);
            await context.SaveChangesAsync(cancellationToken);

            var response = new LoginResponseModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Token = jwt,
                RefreshToken = refreshToken.Token
            };

            return response;
        }

        private async Task ValidateUserInfo(LoginCommand request, User user)
        {
            if (user == null)
            {
                throw new NotFoundException(nameof(User), request.Username);
            }

            var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);

            if (!passwordValid)
            {
                throw new IncorrectPasswordException("The provided password was incorrect.");
            }
        }
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(l => l.Username)
                .NotEmpty()
                .NotNull();

            RuleFor(l => l.Password)
                .NotNull()
                .NotEmpty();
        }
    }
}
