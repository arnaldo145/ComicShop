using ComicShop.Application.Features.Publishers;
using ComicShop.Domain.Features.Publishers;

namespace ComicShop.Tests.Common.TestBuilders.Publishers
{
    public static class PublisherTestBuilder
    {
        public static Publisher Create()
        {
            return new Publisher();
        }

        public static Publisher WithName(this Publisher publisher, string name)
        {
            publisher.SetName(name);
            return publisher;
        }

        public static Publisher WithCountry(this Publisher publisher, string country)
        {
            publisher.SetCountry(country);
            return publisher;
        }

        public static PublisherCreate.Command CreatePublisherCreateCommand()
        {
            return new PublisherCreate.Command();
        }

        public static PublisherCreate.Command WithName(this PublisherCreate.Command command, string name)
        {
            command.Name = name;
            return command;
        }

        public static PublisherCreate.Command WithCountry(this PublisherCreate.Command command, string country)
        {
            command.Country = country;
            return command;
        }
    }
}
