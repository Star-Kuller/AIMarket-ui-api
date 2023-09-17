using FluentValidation;
using IAE.Microservice.Application.Extensions;
using IAE.Microservice.Application.Validators;
using IAE.Microservice.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Exceptions;
using IAE.Microservice.Application.Infrastructure;
using IAE.Microservice.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using ValidationException = IAE.Microservice.Application.Exceptions.ValidationException;

namespace IAE.Microservice.Application.Features.Users
{
    public class ChangeEmailConfirmed
    {
        public class Command : IRequest
        {
            public long Id { get; set; }

            public bool EmailConfirmed { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Id).Id();
            }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly UserManager<User> _userManager;
            private readonly ICurrentUser _currentUser;

            public Handler(UserManager<User> userManager, ICurrentUser currentUser)
            {
                _userManager = userManager;
                _currentUser = currentUser;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                await Transaction.Do(async () =>
                {
                    var user = await _userManager.Users
                                   .WhereForChangeStatusOrEmailConfirmed(request.Id, _currentUser)
                                   .FirstOrDefaultAsync(cancellationToken)
                               ?? throw new NotFoundException(nameof(User), request.Id);

                    if (user.EmailConfirmed == request.EmailConfirmed)
                    {
                        return;
                    }

                    await user.UpdateSecurityStampAsync(_userManager);

                    user.EmailConfirmed = request.EmailConfirmed;
                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        throw new Exceptions.ValidationException(result.GetValidationFailures());
                    }
                });

                return Unit.Value;
            }
        }
    }
}