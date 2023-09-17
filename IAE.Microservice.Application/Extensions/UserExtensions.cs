using FluentValidation;
using IAE.Microservice.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace IAE.Microservice.Application.Extensions
{
    public static class UserExtensions
    {
        public static async Task UpdateSecurityStampAsync(this User user, UserManager<User> userManager)
        {
            if (string.IsNullOrWhiteSpace(user.SecurityStamp) || string.IsNullOrWhiteSpace(user.ConcurrencyStamp))
            {
                var securityStampResult = await userManager.UpdateSecurityStampAsync(user);
                if (!securityStampResult.Succeeded)
                {
                    throw new ValidationException(securityStampResult.GetValidationFailures());
                }
            }
        }
    }
}