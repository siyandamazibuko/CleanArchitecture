using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CleanArchitecture.UnitTests.Common
{
    [TestClass]
    public class AutoMapperTests
    {
        [TestMethod]
        public void Check_AllMappingConfigurations_ShouldAlwaysPass()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(CleanArchitecture.Application.Mappings.UserProfileMapper).Assembly);
            });

            configuration.AssertConfigurationIsValid();
        }
    }
}
