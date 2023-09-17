using IAE.Microservice.Application.Interfaces;
using Moq;
using System;
using IAE.Microservice.Domain.Entities.Users;
using TimeZoneConverter;

namespace IAE.Microservice.Application.Tests.Features.Common
{
    public static class CurrentUserFactory
    {
        public static ICurrentUser CreateAdmin(long id)
        {
            var currentUserMock = new Mock<ICurrentUser>();
            currentUserMock.Setup(x => x.Id).Returns(id);
            currentUserMock.Setup(x => x.IsAdmin).Returns(true);
            currentUserMock.Setup(x => x.Role).Returns(Role.Administrator);
            currentUserMock.Setup(x => x.Language).Returns(Language.English);
            return currentUserMock.Object;
        }
    }
}
