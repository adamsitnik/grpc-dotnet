using NUnit.Framework;
using Moq;
using System;

namespace Grpc.AspNetCore.Tests
{
    public class DefaultGrpcServiceActivatorTests
    {
        public class GrpcService { }

        [Test]
        public void HubCreatedIfNotResolvedFromServiceProvider()
        {
            Assert.NotNull(
                new DefaultGrpcServiceActivator<GrpcService>(Mock.Of<IServiceProvider>()).Create());
        }

        [Test]
        public void HubCanBeResolvedFromServiceProvider()
        {
            var service = Mock.Of<GrpcService>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider
                .Setup(sp => sp.GetService(typeof(GrpcService)))
                .Returns(service);

            Assert.AreSame(service,
                new DefaultGrpcServiceActivator<GrpcService>(mockServiceProvider.Object).Create());
        }
    }
}
