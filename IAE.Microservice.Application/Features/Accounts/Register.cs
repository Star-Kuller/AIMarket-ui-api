using AutoMapper;
using FluentValidation;
using IAE.Microservice.Application.Extensions;
using IAE.Microservice.Application.Interfaces.Bidder;
using IAE.Microservice.Application.Validators;
using IAE.Microservice.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Infrastructure;
using IAE.Microservice.Application.Interfaces;
using IAE.Microservice.Application.Interfaces.Mapping;
using IAE.Microservice.Domain.Entities.Common;

namespace IAE.Microservice.Application.Features.Accounts
{
    /// <summary>
    /// Registering users with the <see cref="Role.Advertiser"/> role
    /// </summary>
    public class Register
    {
        public class Command : IRequest<long>, IHaveCustomMapping
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public Language Language { get; set; } = Language.Russian;

            public void CreateMappings(Profile configuration)
            {
                configuration.CreateMap<Command, User>()
                    .ForMember(entity => entity.UserName, opt => opt.MapFrom(x => x.Email));
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
                RuleFor(x => x.Password).NotEmpty().Password();
                RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
                RuleFor(x => x.Language).IsInEnum();
            }
        }

        public class Handler : IRequestHandler<Command, long>
        {
            private readonly UserManager<User> _userManager;
            private readonly ITradingDeskDbContext _context;
            private readonly IMediator _mediator;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;

            public Handler(
                UserManager<User> userManager, ITradingDeskDbContext context,
                IMediator mediator, IMapper mapper, ILogger<Handler> logger
            )
            {
                _userManager = userManager;
                _context = context;
                _mediator = mediator;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<long> Handle(Command request, CancellationToken cancellationToken)
            {
                await ThrowExceptionWhenValidationError(request);

                var user = _mapper.Map<User>(request);

                await CreateUserAsync(request, user, cancellationToken);

                await _mediator.Send(new RegisterNotification.Command { UserId = user.Id }, cancellationToken);

                return user.Id;
            }

            private async Task ThrowExceptionWhenValidationError(Command request)
            {
            }

            private async Task CreateUserAsync(Command request, User user,
                CancellationToken cancellationToken)
            {
                try
                {
                    await Transaction.Do(async () =>
                    {
                        var result = await _userManager.CreateAsync(user, request.Password);
                        if (!result.Succeeded)
                            throw new Exceptions.ValidationException(result.GetValidationFailures());
                        
                        
                        
                        result = await _userManager.UpdateAsync(user);
                        if (!result.Succeeded)
                            throw new Exceptions.ValidationException(result.GetValidationFailures());
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