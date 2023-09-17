using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IAE.Microservice.Application.Interfaces;
using MediatR;

namespace IAE.Microservice.Application.Features.Chats
{
    public class Enter
    {
        public class Query : IRequest
        {
            public string ChatGuid { get; set; }
        }

        public class Validator
        {
            public Validator()
            {
            }
        }

        public class Handler : IRequestHandler<Query, Unit>
        {
            private readonly IMicriserviceDbContext _context;
            private readonly IMapper _mapper;
            private readonly ICurrentUser _currentUser;

            public Handler(IMicriserviceDbContext context, IMapper mapper, ICurrentUser currentUser)
            {
                _context = context;
                _mapper = mapper;
                _currentUser = currentUser;
            }

            public async Task<Unit> Handle(Query request, CancellationToken cancellationToken)
            {
                return Unit.Value;
            }
        }
    }
}