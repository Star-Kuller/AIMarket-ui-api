using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IAE.Microservice.Application.Interfaces;
using IAE.Microservice.Application.Interfaces.Mapping;
using MediatR;

namespace IAE.Microservice.Application.Features.Chats
{
    public class List
    {
        public class Query : IRequest<Model>
        {
        }

        public class Model : IHaveCustomMapping
        {
            public List<Chats> Chats { get; set; }



            public void CreateMappings(Profile configuration)
            {
            }
        }

        public class Chats
        {
            //TODO Сделать поддержку картинки чата (по готовности хранения картинок)
            public string Name { get; set; }
            public long MessagesCount { get; set; }
        }
        
        public class Validator
        {
            public Validator()
            {
            }
        }

        public class Handler : IRequestHandler<Query, Model>
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

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                return new Model();
            }
        }
    }
}