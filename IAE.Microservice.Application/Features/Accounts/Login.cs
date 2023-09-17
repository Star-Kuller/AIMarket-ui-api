using FluentValidation;
using IAE.Microservice.Domain.Entities.Common;
using IAE.Microservice.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Interfaces;
using IAE.Microservice.Application.Tokens.Models;

namespace IAE.Microservice.Application.Features.Accounts
{
    public class Login
    {
        public class Command : IRequest<string>
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public Language Language { get; set; } = Language.Russian;
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage(x =>
                    x.Language == Language.English
                        ? "Is not a valid email address"
                        : "Адрес почты не валиден");
                RuleFor(x => x.Password).NotEmpty().WithMessage(x =>
                    x.Language == Language.English
                        ? "Password should contain at least one digit"
                        : "Пароль должен содержать хотя бы одну цифру");
                RuleFor(x => x.Language).IsInEnum();
            }
        }

        public class Handler : IRequestHandler<Command, string>
        {
            private readonly UserManager<User> _userManager;
            private readonly ITokenProvider _tokenProvider;

            public Handler(UserManager<User> userManager, ITokenProvider tokenProvider)
            {
                _userManager = userManager;
                _tokenProvider = tokenProvider;
            }

            public async Task<string> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await GetUser(request.Email, request.Password, request.Language);
                var token = _tokenProvider.GetToken(new UserClaims
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = user.UserRoles.Select(ur => ur.Role.Name).First()
                });
                return token;
            }

            private async Task<User> GetUser(string email, string password, Language language)
            {
                var user = await _userManager.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                    throw EmailOrPasswordIsInvalidException(language);

                if (user.Status != Status.Active)
                    throw UserAccountIsNotActiveException(language);

                var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
                if (!isValidPassword)
                    throw EmailOrPasswordIsInvalidException(language);

                return user;
            }

            private static Exceptions.ValidationException EmailOrPasswordIsInvalidException(Language language)
            {
                switch (language)
                {
                    case Language.English:
                        return new Exceptions.ValidationException("",
                            "We couldn`t find a user with this combination of email and password");
                    case Language.Russian:
                    default:
                        return new Exceptions.ValidationException("",
                            "Не нашли пользователя с таким сочетанием почты и пароля");
                }
            }

            private static Exceptions.ValidationException UserAccountIsNotActiveException(Language language)
            {
                switch (language)
                {
                    case Language.English:
                        return new Exceptions.ValidationException("",
                            "User account is not active");
                    default:
                        return new Exceptions.ValidationException("",
                            "Аккаунт пользователя не активирован");
                }
            }
        }
    }
}