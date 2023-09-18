using IAE.Microservice.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Common.Operation;
using IAE.Microservice.Application.Features.Accounts.Update.Models;

namespace IAE.Microservice.Application.Features.Accounts.Update
{
    public class UpdateGet
    {
        public class Response
        {
            public OperationApiResult<UpdateValues> Result { get; set; }
        }

        public class Request : IRequest<Response>, IUpdateUser
        {
            public long? Id { get; set; }

            public string Email { get; set; }
        }

        public class Validator : UpdateUserValidator<Request>
        {
            public Validator()
            {
            }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly UserManager<User> _userManager;

            public Handler(UserManager<User> userManager)
            {
                _userManager = userManager;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var user = await request.GetUserAsync(_userManager);

                var data = new UpdateValues
                {
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    Phone = user.PhoneNumber,
                    FirstName = user.Name,
                    Language = user.Language
                };
                var response = new Response
                {
                    Result = OperationApiResult<UpdateValues>.Success(data, HttpStatusCode.OK)
                };
                return response;
            }
        }
    }
}