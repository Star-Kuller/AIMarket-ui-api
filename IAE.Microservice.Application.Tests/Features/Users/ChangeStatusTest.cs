using FluentAssertions;
using IAE.Microservice.Application.Exceptions;
using IAE.Microservice.Application.Features.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Tests.Features.Common;
using IAE.Microservice.Application.Tests.Infrastructure;
using IAE.Microservice.Domain.Entities.Common;
using IAE.Microservice.Domain.Entities.Users;

namespace IAE.Microservice.Application.Tests.Features.Users
{
    [TestFixture]
    public class ChangeStatusTest
    {

        [Test]
        public async Task ChangeUserStatusToDisabled()
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
                    EmailConfirmed = true,
                    Status = Status.Active
                });

                var sut = new ChangeStatus.Handler(fixture.Context, CurrentUserFactory.CreateAdmin(1),
                    fixture.LoggerFactory.CreateLogger<ChangeStatus.Handler>());

                //ACT
                var result = await sut.Handle(
                    new ChangeStatus.Command { Id = 1, Status = Status.Banned }, CancellationToken.None);

                //ASSERT
                var user = await fixture.Context.Users.FindAsync(1L);
                user.Status.Should().Be(Status.Banned);
            }
        }

        [Test]
        public async Task ChangeUserStatusToEnabled()
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
                    EmailConfirmed = true,
                    Status = Status.Banned
                });

                var sut = new ChangeStatus.Handler(fixture.Context, CurrentUserFactory.CreateAdmin(1),
                    fixture.LoggerFactory.CreateLogger<ChangeStatus.Handler>());

                //ACT
                var result = await sut.Handle(
                    new ChangeStatus.Command { Id = 1, Status = Status.Active }, CancellationToken.None);

                //ASSERT
                var user = await fixture.Context.Users.FindAsync(1L);
                user.Status.Should().Be(Status.Active);
            }
        }

        [Test]
        public async Task ChangeUserStatusToDelete()
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
                    EmailConfirmed = true,
                    Status = Status.Active
                });

                var sut = new ChangeStatus.Handler(fixture.Context, CurrentUserFactory.CreateAdmin(1),
                    fixture.LoggerFactory.CreateLogger<ChangeStatus.Handler>());

                //ACT
                var result = await sut.Handle(
                    new ChangeStatus.Command { Id = 1, Status = Status.Deleted }, CancellationToken.None);

                //ASSERT
                var user = await fixture.Context.Users.FindAsync(1L);
                user.Status.Should().Be(Status.Deleted);

                //check that user not returns from query
                var queryUser = await fixture.Context.Users
                    .Where(u => u.Status != Status.Deleted)
                    .FirstOrDefaultAsync(u => u.Id == 1);
                queryUser.Should().BeNull();
            }
        }

        [Test]
        public async Task ChangeUserStatusWhenNotFound()
        {
            using (var fixture = new QueryTestFixture())
            {
                //ARRANGE                               
                var sut = new ChangeStatus.Handler(fixture.Context, CurrentUserFactory.CreateAdmin(1),
                    fixture.LoggerFactory.CreateLogger<ChangeStatus.Handler>());

                //ACT
                Func<Task> result = async () => await sut.Handle(
                    new ChangeStatus.Command { Id = 1, Status = Status.Active }, CancellationToken.None);

                //ASSERT
                await result.Should().ThrowAsync<NotFoundException>();
            }
        }

        [Test]
        public async Task ChangeUserStatusWhenDeleted()
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
                    EmailConfirmed = true,
                    Status = Status.Deleted,
                    DeletedAt = DateTime.UtcNow
                });

                var sut = new ChangeStatus.Handler(fixture.Context, CurrentUserFactory.CreateAdmin(1),
                    fixture.LoggerFactory.CreateLogger<ChangeStatus.Handler>());

                //ACT
                Func<Task> result = async () => await sut.Handle(
                    new ChangeStatus.Command { Id = 1, Status = Status.Active }, CancellationToken.None);

                //ASSERT
                await result.Should().ThrowAsync<NotFoundException>();
            }
        }
    }
}
