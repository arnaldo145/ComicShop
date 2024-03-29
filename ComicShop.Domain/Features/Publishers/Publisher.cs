﻿using System.Collections.Generic;
using ComicShop.Domain.Features.Comics;

namespace ComicShop.Domain.Features.Publishers
{
    public class Publisher : Entity
    {
        public string Name { get; set; }
        public string Country { get; set; }

        public ICollection<ComicBook> ComicBooks { get; set; }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetCountry(string country)
        {
            Country = country;
        }
    }
}
