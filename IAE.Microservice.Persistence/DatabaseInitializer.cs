using IAE.Microservice.Application.Exceptions;
using IAE.Microservice.Application.Extensions;
using IAE.Microservice.Application.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using IAE.Microservice.Domain.Entities.Users;
using UserLanguage= IAE.Microservice.Domain.Entities.Users.Language;

namespace IAE.Microservice.Persistence
{
    public static class DatabaseInitializer
    {
        private static UserCountry _defaultUserCountry = new UserCountry
        {
            CountryId = 183, // Russia
            TimezoneId = 234 // Europe/Moscow
        };

        public static async Task InitializeAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<TradingDeskDbContext>();
            await context.Database.MigrateAsync();

            await SeedDatabaseDataAsync(context, services);
        }

        private static async Task SeedDatabaseDataAsync(
            TradingDeskDbContext context, IServiceProvider services)
        {
            var adminUserSettings = services.GetService<IOptions<AdminUser>>().Value;

            var adminUser = context.Users.WithRole(Role.Administrator).FirstOrDefault();

            var userManager = services.GetService<UserManager<User>>();

            await Transaction.Do(async () =>
            {
                if (adminUser == null)
                {
                    // create new admin user
                    await CreateUserAsync(adminUserSettings, userManager);
                }
                else
                {
                    await UpdateUserAsync(adminUserSettings, adminUser, userManager);
                }
            });
        }

        private static async Task UpdateUserAsync(
            AdminUser adminUserSettings, 
            User adminUser, UserManager<User> userManager)
        {
            var isValidPassword = await userManager.CheckPasswordAsync(
                                    adminUser, adminUserSettings.Password);

            if (!isValidPassword)
            {
                // update password
                var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(adminUser);
                var resetPasswordResult = await userManager.ResetPasswordAsync(
                    adminUser, passwordResetToken, adminUserSettings.Password);

                if (!resetPasswordResult.Succeeded)
                    throw new ValidationException(resetPasswordResult.GetValidationFailures());
            }

            // update existing user

            adminUser.FirstName = adminUserSettings.FirstName;
            adminUser.LastName = adminUserSettings.LastName;
            adminUser.Email = adminUserSettings.Email;
            adminUser.UserName = adminUserSettings.Email;
            adminUser.Language = adminUserSettings.UserLanguage;

            var result = await userManager.UpdateAsync(adminUser);
            if (!result.Succeeded)
                throw new ValidationException(result.GetValidationFailures());
        }

        private static async Task CreateUserAsync(
            AdminUser adminUserSettings, UserManager<User> userManager)
        {
            var user = new User
            {
                FirstName = adminUserSettings.FirstName,
                LastName = adminUserSettings.LastName,
                Email = adminUserSettings.Email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                UserName = adminUserSettings.Email,
                Language = adminUserSettings.UserLanguage
            };

            var result = await userManager.CreateAsync(user, adminUserSettings.Password);
            if (!result.Succeeded)
                throw new ValidationException(result.GetValidationFailures());

            result = await userManager.AddToRoleAsync(user, Role.Administrator);
            if (!result.Succeeded)
                throw new ValidationException(result.GetValidationFailures());
        }
    }

    public class AdminUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Country { get; set; }
        public string Timezone { get; set; }
        public string Currency { get; set; }
        public string Language { get; set; }

        public Language UserLanguage
        {
            get
            {
                return string.Equals(Language, "RU", StringComparison.InvariantCultureIgnoreCase) ? 
                    Domain.Entities.Users.Language.Russian : 
                    Domain.Entities.Users.Language.English;
            }
        }

    }

    public class UserCountry
    {
        public long CountryId { get; set; }
        public long TimezoneId { get; set; }
    }
}
