using FluentAssertions;
using IAE.Microservice.Application.Features.Users;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using IAE.Microservice.Domain.Entities.Common;

namespace IAE.Microservice.Application.Tests.Features.Users
{
    [TestFixture]
    public class ChangeStatusValidatorTest
    {
        [Test]
        public async Task ChangeUserStatusWhenIdIsEmpty()
        {
            //ARRANGE
            var sut = new ChangeStatus.Validator();

            //ACT
            var result = await sut.ValidateAsync(new ChangeStatus.Command
            {
                Id = 0,
                Status = Status.Active
            });

            //ASSERT
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().PropertyName
                .Should().Be(nameof(ChangeStatus.Command.Id));
        }

        [Test]
        public async Task ChangeUserStatusWhenNewStatusIsInvalid()
        {
            //ARRANGE
            var sut = new ChangeStatus.Validator();

            //ACT
            var result = await sut.ValidateAsync(new ChangeStatus.Command
            {
                Id = 1,
                Status = (Status)12
            });

            //ASSERT
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().PropertyName
                .Should().Be(nameof(ChangeStatus.Command.Status));
        }
    }
}
