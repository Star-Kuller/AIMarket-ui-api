using Microsoft.AspNetCore.Identity;

namespace IAE.Microservice.Domain.Entities.Users
{
    public class RoleClaim : IdentityRoleClaim<long>
    {
        public virtual Role Role { get; set; }
    }
}
