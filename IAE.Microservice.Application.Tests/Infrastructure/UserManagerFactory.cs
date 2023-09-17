using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Collections.Generic;
using IAE.Microservice.Domain.Entities.Users;
using IAE.Microservice.Persistence;
using Microsoft.Extensions.Options;

namespace IAE.Microservice.Application.Tests.Infrastructure
{
    public static class UserManagerFactory
    {
        public static UserManager<User> Create(MicroserviceDbContext dbContext)
        {
            var userStore = new UserStore<
                User, Role, MicroserviceDbContext, long,
                UserClaim, UserRole, UserLogin, UserToken, RoleClaim>(dbContext);
            var passwordHasher = new PasswordHasher<User>();
            var userValidators = new List<UserValidator<User>> { new UserValidator<User>() };
            var logger = new NullLogger<UserManager<User>>();

            return new UserManager<User>(
                store: userStore,
                optionsAccessor: new OptionsWrapper<IdentityOptions>(new IdentityOptions()),
                passwordHasher: passwordHasher,
                userValidators: userValidators,
                passwordValidators: null,
                keyNormalizer: new UpperInvariantLookupNormalizer(),
                errors: new IdentityErrorDescriber(),
                services: null,
                logger: logger);
        }

        public static Mock<UserManager<User>> CreateMock()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
        }

        public static void Destroy(UserManager<User> userManager)
        {
            userManager.Dispose();
        }
    }
}
