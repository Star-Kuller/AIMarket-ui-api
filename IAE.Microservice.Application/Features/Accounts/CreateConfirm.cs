using FluentValidation;
using IAE.Microservice.Application.Extensions;
using IAE.Microservice.Application.Interfaces.Bidder;
using IAE.Microservice.Application.Validators;
using IAE.Microservice.Domain.Entities.Common;
using IAE.Microservice.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Exceptions;
using IAE.Microservice.Application.Infrastructure;

namespace IAE.Microservice.Application.Features.Accounts
{
    public class CreateConfirm
    {
        public class Command : IRequest
        {
            public long UserId { get; set; }
            public string Code { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).Id();
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
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

                if (user == null || user.Status == Status.Deleted)
                    throw new NotFoundException(nameof(User), request.UserId);

                var isValidCode = await _userManager.VerifyUserTokenAsync(
                    user: user,
                    tokenProvider: TokenOptions.DefaultProvider,
                    purpose: UserManager<User>.ResetPasswordTokenPurpose,
                    token: request.Code);

                if (!isValidCode)
                    throw new Exceptions.ValidationException(nameof(request.Code), "invalid code");

                await Transaction.Do(async () =>
                {
                    var addPasswordResult = await _userManager.AddPasswordAsync(user, request.Password);
                    if (!addPasswordResult.Succeeded)
                        throw new Exceptions.ValidationException(addPasswordResult.GetValidationFailures());

                    user.EmailConfirmed = true;
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                        throw new Exceptions.ValidationException(updateResult.GetValidationFailures());
                });

                return Unit.Value;
            }
        }
    }
}
