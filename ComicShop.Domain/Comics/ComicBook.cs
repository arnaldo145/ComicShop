using ComicShop.Domain.Publishers;
using System;

namespace ComicShop.Domain.Comics
{
    public class ComicBook : Entity
    {
        public string Name { get; set; }
        public string ReleaseYear { get; set; }
        public double Price { get; set; }

        public Guid PublisherId { get; set; }
        public Publisher Publisher { get; set; }
    }
}
