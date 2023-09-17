using AutoMapper;
using IAE.Microservice.Application.Common.CacheProviders;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using IAE.Microservice.Domain.Entities.Users;
using IAE.Microservice.Persistence;
using Microsoft.Extensions.Logging;

namespace IAE.Microservice.Application.Tests.Infrastructure
{
    public class QueryTestFixture : IDisposable
    {
        public TradingDeskDbContext Context { get; }
        public IMapper Mapper { get; }
        public UserManager<User> UserManager { get; }
        public Mock<UserManager<User>> UserManagerMock { get; }
        public Mock<IMediator> MediatorMock { get; }
        public LoggerFactory LoggerFactory { get; }
        public Mock<ICacheProvider> CacheProviderMock { get; }

        public QueryTestFixture()
        {
            Context = TradingDeskContextFactory.Create();
            Mapper = AutoMapperFactory.Create();
            UserManager = UserManagerFactory.Create(Context);
            UserManagerMock = UserManagerFactory.CreateMock();
            MediatorMock = new Mock<IMediator>();
            CacheProviderMock = new Mock<ICacheProvider>();
            LoggerFactory = new LoggerFactory();
        }

        public void Dispose()
        {
            TradingDeskContextFactory.Destroy(Context);
            UserManagerFactory.Destroy(UserManager);
            UserManagerFactory.Destroy(UserManagerMock.Object);
        }
    }
}