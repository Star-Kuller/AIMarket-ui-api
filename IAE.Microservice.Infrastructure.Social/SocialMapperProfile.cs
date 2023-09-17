using System.Reflection;
using AutoMapper;
using IAE.Microservice.Application.Infrastructure.AutoMapper;

namespace IAE.Microservice.Infrastructure.Social
{
    public class SocialMapperProfile : Profile
    {
        public SocialMapperProfile()
        {

            LoadCustomMappings();
        }

        private void LoadCustomMappings()
        {
            var mapsFrom = MapperProfileHelper.LoadCustomMappings(Assembly.GetExecutingAssembly());

            foreach (var map in mapsFrom)
            {
                map.CreateMappings(this);
            }
        }
    }
}