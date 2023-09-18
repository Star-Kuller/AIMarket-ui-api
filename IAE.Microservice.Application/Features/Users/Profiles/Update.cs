using AutoMapper;
using FluentValidation;
using IAE.Microservice.Application.Validators;
using IAE.Microservice.Domain.Entities.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Exceptions;
using IAE.Microservice.Application.Interfaces;
using IAE.Microservice.Application.Interfaces.Mapping;
using IAE.Microservice.Domain.Entities.Common;

namespace IAE.Microservice.Application.Features.Users.Profiles
{
    public class Update
    {
        public class Command : IRequest, IHaveCustomMapping
        {
            public long Id { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Gender { get; set; }
            public string Age { get; set; }
            public string About { get; set; }
            public string[] Hobbies { get; set; }

            public void CreateMappings(Profile configuration)
            {
                configuration.CreateMap<Command, User>();
            }
        }

        public class Response
        {
            public long Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
            }
        }

        public class Handler : IRequestHandler<Command, Unit>
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

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_currentUser.IsAdmin)
                    throw new NotFoundException(nameof(User), _currentUser.Id);

                var user = await _context.Users
                               .Where(r => r.Status != Status.Deleted)
                               .FirstOrDefaultAsync(u => u.Id == _currentUser.Id, cancellationToken)
                           ?? throw new NotFoundException(_currentUser.Role, _currentUser.Id);

                _mapper.Map(request, user);

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}