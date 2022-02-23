namespace ComicShop.Application.Features.Identity.DTOs
{
    public class UserTokenDTO
    {
        public string Token { get; private set; }
        public string UserName { get; private set; }

        public UserTokenDTO(string token, string userName)
        {
            Token = token;
            UserName = userName;
        }
    }
}
