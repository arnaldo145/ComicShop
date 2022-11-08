using System;
using System.Threading;
using System.Threading.Tasks;
using ComicShop.Application.Features.Publishers;
using ComicShop.Domain.Features.Publishers;
using ComicShop.Tests.Common.TestBuilders.Publishers;
using ComicShop.UnitTests.Base;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace ComicShop.UnitTests.Features.Publishers
{
    [TestFixture]
    public class PublisherCreateUnitTests : BaseTest
    {
        private PublisherCreate.Handler _handler;
        private Mock<IPublisherRepository> _publisherRepository;

        [SetUp]
        public void Initialize()
        {
            _publisherRepository = new Mock<IPublisherRepository>();

            _handler = new PublisherCreate.Handler(_publisherRepository.Object,
                NullLogger<PublisherCreate.Handler>.Instance,
                Mapper);
        }

        [Test]
        public async Task CriarEditora_Deve_ObterSucesso()
        {
            // Arrange
            var expectedGuid = Guid.NewGuid();
            var command = PublisherTestBuilder.CreatePublisherCreateCommand()
                .WithName("Marvel")
                .WithCountry("EUA");

            _publisherRepository.Setup(x => x.HasAnyAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            _publisherRepository.Setup(x => x.AddAsync(It.IsAny<Publisher>()))
                .ReturnsAsync(expectedGuid);

            // Act
            var response = await _handler.Handle(command, It.IsAny<CancellationToken>());

            // Assert
            response.Should().Be(expectedGuid);
        }

        [Test]
        public async Task CriarEditora_Com_NomeVazio_Deve_RetornarFalhaNaValidacao()
        {
            // Arrange
            var expectedErrorsCount = 1;

            var command = PublisherTestBuilder.CreatePublisherCreateCommand()
                .WithName(string.Empty)
                .WithCountry("EUA");

            // Act
            var response = command.Validate();

            // Assert
            response.IsValid.Should().BeFalse();
            response.Errors.Count.Should().Be(expectedErrorsCount);
        }

        [Test]
        public async Task CriarEditora_Com_PaisVazio_Deve_RetornarFalhaNaValidacao()
        {
            // Arrange
            var expectedErrorsCount = 1;

            var command = PublisherTestBuilder.CreatePublisherCreateCommand()
                .WithName("DC")
                .WithCountry(string.Empty);

            // Act
            var response = command.Validate();

            // Assert
            response.IsValid.Should().BeFalse();
            response.Errors.Count.Should().Be(expectedErrorsCount);
        }
    }
}
