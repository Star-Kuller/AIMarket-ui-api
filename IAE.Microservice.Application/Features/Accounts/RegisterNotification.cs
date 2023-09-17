using IAE.Microservice.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Exceptions;
using IAE.Microservice.Application.Features.Accounts.Notifications;
using IAE.Microservice.Application.Interfaces;

namespace IAE.Microservice.Application.Features.Accounts
{
    public class RegisterNotification : INotification
    {
        public class Command : IRequest
        {
            public long UserId { get; set; }
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

            public async Task<Unit> Handle(Command notification, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByIdAsync(notification.UserId.ToString())
                    ?? throw new NotFoundException(nameof(User), notification.UserId);

                if (user.EmailConfirmed)
                    throw new ValidationException("", "account already confirmed");

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var message = _messageProvider.GetRegisterMessage(user, code);

                await _notificationSender.SendAsync(message);

                return default;
            }
        }
    }
}
