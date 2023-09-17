using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IAE.Microservice.Application.Interfaces;
using IAE.Microservice.Application.Interfaces.Mapping;
using MediatR;
using Newtonsoft.Json;

namespace IAE.Microservice.Application.Features.Chats
{
    public class ChatHistory
    {
        public class Query : IRequest<Model>
        {
            [JsonIgnore]
            public long Id { get; set; }
            public long MessagesCount { get; set; }
        }

        public class Model : IHaveCustomMapping
        {
            //TODO Сделать поддержку картинки чата (по готовности хранения картинок)
            public string Name { get; set; }
            public long MessagesCount { get; set; }
            public List<Message> Messages { get; set; }

            public void CreateMappings(Profile configuration)
            {
            }
        }

        public class Message
        {
            public long Id { get; set; }
            public User User { get; set; }
            public DateTime Date { get; set; }
            public string Massage { get; set; }
        }
        
        public class User
        {
            //TODO Сделать поддержку картинки чата (по готовности хранения картинок)
            public string Name { get; set; }
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