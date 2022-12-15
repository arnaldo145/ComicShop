using System;
using ComicShop.Domain.Features.Publishers;

namespace ComicShop.Domain.Features.Comics
{
    public class ComicBook : Entity
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
        public DateTime ReleaseDate { get; set; }

        public Guid PublisherId { get; set; }
        public Publisher Publisher { get; set; }
    }
}
