using System.Collections.Generic;
using System.Linq;
using ComicShop.Domain.Features.Identity;

namespace ComicShop.Infra.Data.Features.Identity
{
    public class UserRepository : IUserRepository
    {
        public static IList<User> Users = new List<User>
        {
            new User
            {
                Type = 1,
                Email = "arnaldomadeiradev@outlook.com",
                Name = "Arnaldo",
                Password = "SenhaDoArnaldo"
            }
        };

        public User GetByEmail(string email)
        {
            return Users.Where(x => x.Email.ToLower().Equals(email.ToLower()))
                .SingleOrDefault();
        }
    }
}
