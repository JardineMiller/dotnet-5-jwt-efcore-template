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
        private readonly ApplicationDbContext _context;
        private readonly TokenFactory _tokenFactory;
        private readonly UserManager<User> _userManager;

        public LoginCommandHandler(UserManager<User> userManager, TokenFactory tokenFactory,
            ApplicationDbContext context)
        {
            this._userManager = userManager;
            this._tokenFactory = tokenFactory;
            this._context = context;
        }

        public async Task<LoginResponseModel> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await this._userManager.Users
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.UserName == request.Username, cancellationToken);

            await ValidateUserInfo(request, user);

            var jwt = this._tokenFactory.GenerateJwtToken(user.Id, user.UserName);
            var refreshToken = this._tokenFactory.GenerateRefreshToken();

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

            this._context.Update(user);
            await this._context.SaveChangesAsync(cancellationToken);

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

            var passwordValid = await this._userManager.CheckPasswordAsync(user, request.Password);

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