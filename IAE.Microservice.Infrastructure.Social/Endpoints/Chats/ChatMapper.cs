using AutoMapper;
using IAE.Microservice.Application.Features.Chats;
using IAE.Microservice.Application.Interfaces.Mapping;

namespace IAE.Microservice.Infrastructure.Social.Endpoints.Chats
{
    public class ChatMapper : IHaveCustomMapping
    {
        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<Create.Command, ChatVm.CreateOrUpdateRequest>();

            configuration.CreateMap<ChatVm.CreateOrUpdateResponse, Create.Response>();
            
        }
    }
}