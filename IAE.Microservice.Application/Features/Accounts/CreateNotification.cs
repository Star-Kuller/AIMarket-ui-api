using IAE.Microservice.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using IAE.Microservice.Application.Exceptions;
using IAE.Microservice.Application.Features.Accounts.Notifications;
using IAE.Microservice.Application.Interfaces;

namespace IAE.Microservice.Application.Features.Accounts
{
    public class CreateNotification : INotification
    {
        public long UserId { get; set; }

        public class Handler : INotificationHandler<CreateNotification>
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

            public async Task Handle(CreateNotification notification, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByIdAsync(notification.UserId.ToString())
                    ?? throw new NotFoundException(nameof(User), notification.UserId);

                var code = await _userManager.GenerateUserTokenAsync(
                    user, TokenOptions.DefaultProvider, UserManager<User>.ResetPasswordTokenPurpose);

                var encodedCode = HttpUtility.UrlEncode(code);
                var message = _messageProvider.GetCreateMessage(user, encodedCode);

                await _notificationSender.SendAsync(message);
            }
        }
    }
}
