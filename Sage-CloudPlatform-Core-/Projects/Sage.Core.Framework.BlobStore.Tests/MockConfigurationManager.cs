using Moq;
using Sage.Core.Framework.Configuration;

namespace Sage.Core.Framework.BlobStore.Tests
{
    class MockConfigurationManager 
    {
        public static IConfigurationManager Create()
        {
            var mockConfiguration = new Mock<IConfigurationManager>();

            var systemConfiguration = new Mock<IConfiguration>();
            var applicationConfiguration = new Mock<IConfiguration>();
            var config = new Mock<IConfigurationManager>();
           
            mockConfiguration.Setup(ctx => ctx.ApplicationConfiguration).Returns(applicationConfiguration.Object);
            mockConfiguration.Setup(ctx => ctx.SystemConfiguration).Returns(systemConfiguration.Object);

            return mockConfiguration.Object;
        }

    }
}
