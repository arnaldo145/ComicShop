using AutoMapper;
using ComicShop.Application.Features.Publishers;
using ComicShop.Domain.Features.Publishers;

namespace ComicShop.WebApi.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PublisherCreate.Command, Publisher>();
        }
    }
}
