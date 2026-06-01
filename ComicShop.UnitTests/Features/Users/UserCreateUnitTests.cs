using System;
using System.Threading;
using System.Threading.Tasks;
using ComicShop.Application.Features.Users;
using ComicShop.Application.Features.Users.Services;
using ComicShop.Domain.Features.Users;
using ComicShop.Tests.Common.TestBuilders.Users;
using ComicShop.UnitTests.Base;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace ComicShop.UnitTests.Features.Users
{
    [TestFixture]
    public class UserCreateUnitTests : BaseTest
    {
        private Mock<IPasswordService> _passwordService;
        private Mock<IUserRepository> _userRepository;
        private UserCreate.Handler _handler;

        [SetUp]
        public void Initialize()
        {
            _passwordService = new Mock<IPasswordService>();
            _userRepository = new Mock<IUserRepository>();

            _passwordService.Setup(ps => ps.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
                .Returns("hashed-password");

            _handler = new UserCreate.Handler(_userRepository.Object,
                _passwordService.Object,
                NullLogger<UserCreate.Handler>.Instance,
                Mapper);
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
            response.IsSuccess.Should().BeTrue();
            response.Success.Should().BeOfType<User>();
            response.Success.Name.Should().Be("Default");

        }
    }
}
