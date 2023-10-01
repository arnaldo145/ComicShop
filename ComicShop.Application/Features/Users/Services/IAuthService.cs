using System;
using ComicShop.Domain.Features.Users;
using ComicShop.Infra.Structs;

namespace ComicShop.Application.Features.Users.Services
{
    public interface IAuthService
    {
        Result<Exception, string> GenerateToken(User user);
    }
}
