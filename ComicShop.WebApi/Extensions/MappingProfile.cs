using AutoMapper;
using ComicShop.Application.Features.Comics;
using ComicShop.Application.Features.Publishers;
using ComicShop.Application.Features.Users;
using ComicShop.Domain.Features.Comics;
using ComicShop.Domain.Features.Publishers;
using ComicShop.Domain.Features.Users;
using ComicShop.WebApi.Controllers.v1.Comics.ViewModels;
using ComicShop.WebApi.Controllers.v1.Publishers.ViewModels;

namespace ComicShop.WebApi.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PublisherCreate.Command, Publisher>();

            CreateMap<Publisher, PublisherResumeViewModel>();

            CreateMap<UserCreate.Command, User>();

            CreateMap<ComicBookCreate.Command, ComicBook>();

            CreateMap<ComicBook, ComicBookResumeViewModel>()
                .ForMember(x => x.PublisherName, opt => opt.MapFrom(x => x.Publisher.Name));
        }
    }
}
