using System;

namespace ComicShop.WebApi.Controllers.v1.Comics.ViewModels
{
    public class ComicBookResumeViewModel
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string PublisherName { get; set; }
        public int Amount { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
