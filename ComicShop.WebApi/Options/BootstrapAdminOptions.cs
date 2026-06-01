namespace ComicShop.WebApi.Options
{
    public class BootstrapAdminOptions
    {
        public const string SectionName = "Bootstrap:Admin";

        public string Name { get; set; } = "Admin";
        public string Email { get; set; }
        public string Password { get; set; }

        public bool IsConfigured => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
    }
}
