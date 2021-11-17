using System;

namespace ComicShop.WebApi.Controllers.v1.Publishers.ViewModels
{
    public class PublisherResumeViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
    }
}
