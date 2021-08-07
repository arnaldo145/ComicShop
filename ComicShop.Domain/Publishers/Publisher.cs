using ComicShop.Domain.Comics;
using System;
using System.Collections.Generic;

namespace ComicShop.Domain.Publishers
{
    public class Publisher : Entity
    {
        public string Name { get; set; }
        public string Country { get; set; }

        public ICollection<ComicBook> ComicBooks { get; set; }
    }
}
