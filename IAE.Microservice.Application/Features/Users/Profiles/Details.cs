using AutoMapper;
using IAE.Microservice.Application.Interfaces.Mapping;
using IAE.Microservice.Domain.Entities.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Exceptions;
using IAE.Microservice.Application.Interfaces;
using IAE.Microservice.Domain.Entities.Common;

namespace IAE.Microservice.Application.Features.Users.Profiles
{
    public class Details
    {
        public class Query : IRequest<Model>
        {
        }

        public class Model
        {
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public Language Language { get; set; }
            
            public void CreateMappings(Profile configuration)
            {
                configuration.CreateMap<User, Model>();
            }
        }

        public class Handler : IRequestHandler<Query, Model>
        {
            private readonly ITradingDeskDbContext _context;
            private readonly IMapper _mapper;
            private readonly ICurrentUser _currentUser;

            public Handler(
                ITradingDeskDbContext context, IMapper mapper, ICurrentUser currentUser)
            {
                _context = context;
                _mapper = mapper;
                _currentUser = currentUser;
            }

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_currentUser.IsAdmin)
                    throw new NotFoundException(nameof(User), _currentUser.Id);

                var user = await _context.Users
                               .Where(r => r.Status != Status.Deleted)
                               .Where(u => u.Id == _currentUser.Id)
                               .FirstOrDefaultAsync(cancellationToken)
                           ?? throw new NotFoundException(_currentUser.Role, _currentUser.Id);

                return _mapper.Map<Model>(user);
            }
        }
    }
}