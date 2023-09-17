using FluentValidation;
using IAE.Microservice.Application.Extensions;
using IAE.Microservice.Application.Validators;
using IAE.Microservice.Domain.Entities.Common;
using IAE.Microservice.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using ValidationException = IAE.Microservice.Application.Exceptions.ValidationException;

namespace IAE.Microservice.Application.Features.Accounts
{
    public class RegisterConfirm
    {
        public class Command : IRequest
        {
            public long UserId { get; set; }
            public string Code { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).Id();
                RuleFor(x => x.Code).NotEmpty();
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
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());

                if (user == null || user.Status == Status.Deleted)
                    throw new Exceptions.ValidationException(nameof(User), "invalid userId");

                if (user.EmailConfirmed)
                    throw new Exceptions.ValidationException("", "account already confirmed");

                var result = await _userManager.ConfirmEmailAsync(user, request.Code);

                if (!result.Succeeded)
                    throw new Exceptions.ValidationException(result.GetValidationFailures());

                return Unit.Value;
            }
        }
    }
}
