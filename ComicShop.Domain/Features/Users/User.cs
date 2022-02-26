namespace ComicShop.Domain.Features.Users
{
    public class User : Entity
    {
        public int Type { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
