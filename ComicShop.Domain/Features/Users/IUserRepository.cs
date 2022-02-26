﻿using System.Threading.Tasks;

namespace ComicShop.Domain.Features.Users
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
    }
}
