using Microsoft.AspNetCore.Identity;

namespace IAE.Microservice.Domain.Entities.Users
{
    public class UserRole : IdentityUserRole<long>
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
