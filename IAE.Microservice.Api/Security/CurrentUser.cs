using IAE.Microservice.Application.Interfaces;
using System;
using IAE.Microservice.Domain.Entities.Users;
using Users = IAE.Microservice.Domain.Entities.Users;

namespace IAE.Microservice.Api.Security
{
    public class CurrentUser : ICurrentUser
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public Language Language { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }

        public bool IsAdmin => Role == Domain.Entities.Users.Role.Administrator;
    }
}
