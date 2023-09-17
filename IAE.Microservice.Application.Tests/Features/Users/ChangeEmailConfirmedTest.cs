using FluentAssertions;
using IAE.Microservice.Application.Exceptions;
using IAE.Microservice.Application.Features.Users;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Tests.Features.Common;
using IAE.Microservice.Application.Tests.Infrastructure;
using IAE.Microservice.Domain.Entities.Common;
using IAE.Microservice.Domain.Entities.Users;

namespace IAE.Microservice.Application.Tests.Features.Users
{
    [TestFixture]
    public class ChangeEmailConfirmedTest
    {
        [Test]
        public async Task ChangeUserEmailConfirmed()
        {
            using (var fixture = new QueryTestFixture())
            {
                //ARRANGE               
                await fixture.UserManager.CreateAsync(new User
                {
                    Id = 1,
                    Email = "agency1@test.com",
                    UserName = "agency1@test.com",
                    PhoneNumber = "8304561237",
                    EmailConfirmed = false,
                    Status = Status.Active
                });

                var sut = new ChangeEmailConfirmed.Handler(fixture.UserManager, CurrentUserFactory.CreateAdmin(1));

                //ACT
                var result = await sut.Handle(new ChangeEmailConfirmed.Command
                {
                    Id = 1,
                    EmailConfirmed = true
                }, CancellationToken.None);

                //ASSERT
                var user = await fixture.Context.Users.FindAsync(1L);
                user.EmailConfirmed.Should().Be(true);
            }
        }

        [Test]
        public async Task ChangeUserEmailConfirmedWhenNotFound()
        {
            using (var fixture = new QueryTestFixture())
            {
                //ARRANGE                               
                var sut = new ChangeEmailConfirmed.Handler(fixture.UserManager, CurrentUserFactory.CreateAdmin(1));

                //ACT
                Func<Task> result = async () => await sut.Handle(new ChangeEmailConfirmed.Command
                {
                    Id = 1,
                    EmailConfirmed = true
                }, CancellationToken.None);

                //ASSERT
                await result.Should().ThrowAsync<NotFoundException>();
            }
        }

        [Test]
        public async Task ChangeUserEmailConfirmedWhenDeleted()
        {
            using (var fixture = new QueryTestFixture())
            {
                //ARRANGE               
                await fixture.UserManager.CreateAsync(new User
                {
                    Id = 1,
                    Email = "agency1@test.com",
                    UserName = "agency1@test.com",
                    PhoneNumber = "8304561237",
                    EmailConfirmed = false,
                    Status = Status.Deleted,
                    DeletedAt = DateTime.UtcNow
                });

                var sut = new ChangeEmailConfirmed.Handler(fixture.UserManager, CurrentUserFactory.CreateAdmin(1));

                //ACT
                Func<Task> result = async () => await sut.Handle(new ChangeEmailConfirmed.Command
                {
                    Id = 1,
                    EmailConfirmed = true
                }, CancellationToken.None);

                //ASSERT
                await result.Should().ThrowAsync<NotFoundException>();
            }
        }
    }
}