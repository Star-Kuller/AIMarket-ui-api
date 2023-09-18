using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using IAE.Microservice.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IAE.Microservice.Application.Features.Chats
{
    public class EditMessage
    {
        public class Command : IRequest<Response>
        {
            public long MessageId { get; set; }
            public string Message { get; set; }
        }
        
        public class Response : IRequest<Response>
        {
            public long Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
            }
        }

        public class Handler : IRequestHandler<Command, Response>
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

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                return new Response();
            }
        }
    }
}