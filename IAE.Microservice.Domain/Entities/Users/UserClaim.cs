using Microsoft.AspNetCore.Identity;

namespace IAE.Microservice.Domain.Entities.Users
{
    public class UserClaim : IdentityUserClaim<long>
    {
        public virtual User User { get; set; }
    }
}
