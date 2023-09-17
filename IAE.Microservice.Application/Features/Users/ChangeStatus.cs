using FluentValidation;
using IAE.Microservice.Application.Extensions;
using IAE.Microservice.Application.Interfaces.Bidder;
using IAE.Microservice.Application.Validators;
using IAE.Microservice.Domain.Entities.Common;
using IAE.Microservice.Domain.Entities.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Exceptions;
using IAE.Microservice.Application.Infrastructure;
using IAE.Microservice.Application.Interfaces;
using IAE.Microservice.Application.Common.Operation;


namespace IAE.Microservice.Application.Features.Users
{
    public class ChangeStatus
    {
        public class Command : IRequest<Unit>
        {
            public long Id { get; set; }
            public Status Status { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Id).Id();
                RuleFor(x => x.Status).IsInEnum();
            }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly ITradingDeskDbContext _context;
            private readonly ICurrentUser _currentUser;
            private readonly ILogger<Handler> _logger;

            public Handler(ITradingDeskDbContext context, ICurrentUser currentUser,
                ILogger<Handler> logger)
            {
                _context = context;
                _currentUser = currentUser;
                _logger = logger;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                               .Where(u => u.Status != Status.Deleted)
                               .WhereForChangeStatusOrEmailConfirmed(request.Id, _currentUser)
                               .WithRole()
                               .FirstOrDefaultAsync(cancellationToken)
                           ?? throw new NotFoundException(nameof(User), request.Id);

                user.ChangeStatus(request.Status);

                await ChangeStatusAsync(user, request, cancellationToken);

                return Unit.Value;
            }

            private async Task ChangeStatusAsync(User user, Command request, CancellationToken cancellationToken)
            {
                try
                {
                    await Transaction.Do(async () =>
                    {
                        //Here is a request to other services
                        await _context.SaveChangesAsync(cancellationToken);
                    });
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    throw;
                }
            }
        }
    }
}