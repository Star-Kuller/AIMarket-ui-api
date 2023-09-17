using FluentValidation;
using IAE.Microservice.Application.Extensions;
using IAE.Microservice.Application.Validators;
using IAE.Microservice.Domain.Entities.Common;
using IAE.Microservice.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Exceptions;
using ValidationException = IAE.Microservice.Application.Exceptions.ValidationException;

namespace IAE.Microservice.Application.Features.Accounts
{
    public class RecoveryPasswordConfirm
    {
        public class Command : IRequest
        {
            public string Email { get; set; }
            public string Code { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
                RuleFor(x => x.Code).NotEmpty();
                RuleFor(x => x.Password).NotEmpty().Password();
                RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);
            }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly UserManager<User> _userManager;

            public Handler(UserManager<User> userManager)
            {
                _userManager = userManager;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null || user.Status == Status.Deleted)
                    throw new NotFoundException(nameof(User), request.Email);

                if (user.Status == Status.Banned)
                    throw new Exceptions.ValidationException("", "account is banned");

                var result = await _userManager.ResetPasswordAsync(user, request.Code, request.Password);
                if (!result.Succeeded)
                    throw new Exceptions.ValidationException(result.GetValidationFailures());

                if (user.EmailConfirmed)
                    return Unit.Value;

                user.EmailConfirmed = true;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    throw new Exceptions.ValidationException(result.GetValidationFailures());

                return Unit.Value;
            }
        }
    }
}