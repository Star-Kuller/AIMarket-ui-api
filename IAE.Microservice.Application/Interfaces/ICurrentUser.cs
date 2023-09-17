using IAE.Microservice.Domain.Entities.Users;
using System;

namespace IAE.Microservice.Application.Interfaces
{
    public interface ICurrentUser
    {
        long Id { get; set; }
        string Email { get; set; }
        string Role { get; set; }
        Language Language { get; set; }
        string IpAddress { get; set; }
        string UserAgent { get; set; }


        bool IsAdmin { get; }
    }
}