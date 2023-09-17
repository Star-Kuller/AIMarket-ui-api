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
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public Language Language { get; set; }

            public void CreateMappings(Profile configuration)
            {
                configuration.CreateMap<Command, User>();
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.FirstName).NotEmpty().MaximumLength(255);
                RuleFor(x => x.LastName).NotEmpty().MaximumLength(255);
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