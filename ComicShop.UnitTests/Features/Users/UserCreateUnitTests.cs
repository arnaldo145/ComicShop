using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ComicShop.Application.Features.Users;
using ComicShop.Domain.Features.Users;
using ComicShop.Tests.Common.TestBuilders.Users;
using ComicShop.WebApi.Extensions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ComicShop.UnitTests.Features.Users
{
    [TestFixture]
    public class UserCreateUnitTests
    {
        private Mock<IUserRepository> _userRepository;
        private IMapper _mapper;

        private UserCreate.Handler _handler;

        [SetUp]
        public void Initialize()
        {
            _userRepository = new Mock<IUserRepository>();

            var profile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            _mapper = new Mapper(configuration);

            _handler = new UserCreate.Handler(_userRepository.Object, _mapper);
        }

        [Test]
        public async Task CriarUsuario_Deve_ObterSucesso()
        {
            // Arrange
            var userCreateCommand = UserTestBuilder.CreateUserCommand()
                .WithName("Default")
                .WithEmail("default@default.com")
                .WithPassword("password")
                .WithType(1);

            var user = UserTestBuilder.CreateUser()
                .WithId(Guid.NewGuid())
                .WithName("Default")
                .WithEmail("default@default.com")
                .WithPassword("password")
                .WithType(1);

            _userRepository.Setup(ur => ur.Add(It.IsAny<User>()))
                .Returns(user);

            // Act
            var response = await _handler.Handle(userCreateCommand, It.IsAny<CancellationToken>());

            // Assert
            response.Should().BeOfType<User>();
            response.Name.Should().Be("Default");

        }
    }
}
