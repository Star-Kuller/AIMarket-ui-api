using IAE.Microservice.Common;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using IAE.Microservice.Domain.Entities.Common;


namespace IAE.Microservice.Domain.Entities.Users
{
    public class User : IdentityUser<long>
    {
        

        public string Name { get; set; }
        public long SocialId { get; set; }

        public Language Language { get; set; }
        public Status Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        
        public virtual IList<UserClaim> Claims { get; set; }
        public virtual IList<UserLogin> Logins { get; set; }
        public virtual IList<UserToken> Tokens { get; set; }
        public virtual IList<UserRole> UserRoles { get; set; }
        public virtual IList<User> Users { get; private set; }

        public User()
        {
            Language = Language.English;
            Status = Status.Active;
            CreatedAt = SystemTime.DateTime.UtcNow;
            Claims = new List<UserClaim>();
            Logins = new List<UserLogin>();
            Tokens = new List<UserToken>();
            UserRoles = new List<UserRole>();
            Users = new List<User>();
        }

        public string RoleName
        {
            get
            {
                if (UserRoles == null)
                {
                    return null;
                }

                return UserRoles
                    .Where(ur => ur.Role != null)
                    .Select(ur => ur.Role.Name)
                    .FirstOrDefault();
            }
        }
        
        
        public bool IsAdmin()
        {
            return RoleName == Role.Administrator;
        }
        
        public void Activate() => Status = Status.Active;

        public void Ban() => Status = Status.Banned;

        public void Delete()
        {
            Status = Status.Deleted;
            DeletedAt = SystemTime.DateTime.UtcNow;
        }

        public void ChangeStatus(Status newStatus)
        {
            switch (newStatus)
            {
                case Status.Active:
                    Activate();
                    break;
                case Status.Banned:
                    Ban();
                    break;
                case Status.Deleted:
                    Delete();
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}