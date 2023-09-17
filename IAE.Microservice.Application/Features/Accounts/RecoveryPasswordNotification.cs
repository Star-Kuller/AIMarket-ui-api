using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using IAE.Microservice.Application.Features.Accounts.Notifications;
using IAE.Microservice.Application.Interfaces;
using IAE.Microservice.Application.Extensions;
using IAE.Microservice.Domain.Entities.Common;
using IAE.Microservice.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ValidationException = IAE.Microservice.Application.Exceptions.ValidationException;

namespace IAE.Microservice.Application.Features.Accounts
{
    public class RecoveryPasswordNotification
    {
        public class Command : IRequest
        {
            public string Email { get; set; }
            public Language Language { get; set; } = Language.Russian;
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly UserManager<User> _userManager;
            private readonly INotificationSender _notificationSender;
            private readonly MessageProvider _messageProvider;

            public Handler(
                UserManager<User> userManager,
                INotificationSender notificationSender,
                MessageProvider messageProvider)
            {
                _userManager = userManager;
                _notificationSender = notificationSender;
                _messageProvider = messageProvider;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                    throw new Exceptions.ValidationException("", request.Language == Language.English
                        ? "User not found"
                        : "Пользователь не найден");
                
                switch (user.Status)
                {
                    case Status.Deleted:
                        throw new Exceptions.ValidationException("", request.Language == Language.English
                            ? "User is deleted"
                            : "Пользователь удален");
                    case Status.Banned:
                        throw new Exceptions.ValidationException("", request.Language == Language.English
                            ? "User is banned"
                            : "Пользователь заблокирован");
                }

                await user.UpdateSecurityStampAsync(_userManager);

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var message = _messageProvider.GetRecoveryPasswordMessage(user, code);
                await _notificationSender.SendAsync(message);

                return default;
            }
        }
    }
}