using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace IAE.Microservice.Domain.Entities.Users
{
    public class Role : IdentityRole<long>
    {
        public const string Administrator = "Administrator";
        public const string Member = "Default user";
        public const string AdministratorRu = "Администратор";
        public const string MemberRu = "Обычный пользователь";

        public string NameRu { get; set; }

        public virtual IList<UserRole> UserRoles { get; set; }
        public virtual IList<RoleClaim> RoleClaims { get; set; }

        public Role()
        {
            UserRoles = new List<UserRole>();
            RoleClaims = new List<RoleClaim>();
        }
    }
}
