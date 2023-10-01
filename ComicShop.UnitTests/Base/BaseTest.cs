using AutoMapper;
using ComicShop.WebApi.Extensions;
using NUnit.Framework;

namespace ComicShop.UnitTests.Base
{
    public class BaseTest
    {
        public IMapper Mapper { get; private set; }

        [OneTimeSetUp]
        public void BaseSetup()
        {
            var profile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            Mapper = new Mapper(configuration);
        }
    }
}
