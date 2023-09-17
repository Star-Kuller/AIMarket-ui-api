using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using IAE.Microservice.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IAE.Microservice.Application.Features.Chats
{
    public class Create
    {
        public class Command : IRequest<long>
        {
            //TODO Сделать поддержку картинки чата (по готовности хранения картинок)
            public string Name { get; set; }
            public List<long> Participants { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
            }
        }

        public class Handler : IRequestHandler<Command, long>
        {
            private readonly IMicriserviceDbContext _context;
            private readonly ICurrentUser _currentUser;
            private readonly ILogger<Handler> _logger;

            public Handler(IMicriserviceDbContext context, ICurrentUser currentUser,
                ILogger<Handler> logger)
            {
                _context = context;
                _currentUser = currentUser;
                _logger = logger;
            }

            public async Task<long> Handle(Command request, CancellationToken cancellationToken)
            {
                return 0;
            }
        }
    }
}