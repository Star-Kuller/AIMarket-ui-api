using FluentValidation;
using IAE.Microservice.Application.Extensions;
using IAE.Microservice.Domain.Entities.Common;
using IAE.Microservice.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Exceptions;
using IAE.Microservice.Application.Features.Accounts.Notifications;
using IAE.Microservice.Application.Interfaces;
using ValidationException = IAE.Microservice.Application.Exceptions.ValidationException;

namespace IAE.Microservice.Application.Features.Accounts
{
    public class RecoveryPassword
    {
        public class Command : IRequest<Model>
        {
            public string Email { get; set; }
        }

        public class Model
        {
            public string Code { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
            }
        }

        public class Handler : IRequestHandler<Command, Model>
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

            public async Task<Model> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null || user.Status == Status.Deleted)
                    throw new NotFoundException(nameof(User), request.Email);

                if (user.Status == Status.Banned)
                    throw new Exceptions.ValidationException("", "account is banned");

                await user.UpdateSecurityStampAsync(_userManager);

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var message = _messageProvider.GetRecoveryPasswordMessage(user, code);
                await _notificationSender.SendAsync(message);

                return new Model { Code = code };
            }
        }
    }
}