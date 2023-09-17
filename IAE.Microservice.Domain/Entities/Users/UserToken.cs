using Microsoft.AspNetCore.Identity;

namespace IAE.Microservice.Domain.Entities.Users
{
    public class UserToken : IdentityUserToken<long>
    {
        public virtual User User { get; set; }
    }
}
