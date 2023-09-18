using AutoMapper;
using IAE.Microservice.Application.Features.Users.Profiles;
using IAE.Microservice.Application.Interfaces.Mapping;

namespace IAE.Microservice.Infrastructure.Social.Endpoints.Users
{
    public class UserMapper : IHaveCustomMapping
    {
        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<Update.Command, UserVm.CreateOrUpdateRequest>();

            configuration.CreateMap<UserVm.CreateOrUpdateResponse, Update.Response>();

        }
    }
}