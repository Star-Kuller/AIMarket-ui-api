using FluentValidation;
using IAE.Microservice.Application.Extensions;
using IAE.Microservice.Application.Validators;
using IAE.Microservice.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Exceptions;
using IAE.Microservice.Application.Features.Accounts.Notifications;
using IAE.Microservice.Application.Infrastructure;
using IAE.Microservice.Application.Interfaces;
using ValidationException = IAE.Microservice.Application.Exceptions.ValidationException;

namespace IAE.Microservice.Application.Features.Accounts
{
    public class ChangePassword
    {
        public class Command : IRequest
        {
            public string CurrentPassword { get; set; }
            public string NewPassword { get; set; }
            public string ConfirmNewPassword { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.CurrentPassword).NotEmpty();
                RuleFor(x => x.NewPassword).NotEmpty().Password();
                RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword)
                    .WithMessage("Password and password confirmation don't match");
            }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly UserManager<User> _userManager;
            private readonly INotificationSender _notificationSender;
            private readonly ICurrentUser _currentUser;
            private readonly MessageProvider _messageProvider;

            public Handler(
                UserManager<User> userManager, INotificationSender notificationSender,
                ICurrentUser currentUser, MessageProvider messageProvider)
            {
                _userManager = userManager;
                _notificationSender = notificationSender;
                _currentUser = currentUser;
                _messageProvider = messageProvider;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                await Transaction.Do(async () =>
                {
                    var user = await _userManager.FindByIdAsync(_currentUser.Id.ToString())
                    ?? throw new NotFoundException(nameof(User), _currentUser.Id);

                    var result = await _userManager.ChangePasswordAsync(
                        user, request.CurrentPassword, request.NewPassword);
                    if (!result.Succeeded)
                        throw new Exceptions.ValidationException(result.GetValidationFailures());

                    var message = _messageProvider.GetChangePasswordMessage(user);
                    await _notificationSender.SendAsync(message);
                });

                return Unit.Value;
            }
        }
    }
}
