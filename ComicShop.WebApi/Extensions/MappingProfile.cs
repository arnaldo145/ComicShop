using AutoMapper;
using ComicShop.Application.Features.Publishers;
using ComicShop.Domain.Features.Publishers;
using ComicShop.WebApi.Controllers.v1.Publishers.ViewModels;

namespace ComicShop.WebApi.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PublisherCreate.Command, Publisher>();
            CreateMap<Publisher, PublisherResumeViewModel>();
        }
    }
}
