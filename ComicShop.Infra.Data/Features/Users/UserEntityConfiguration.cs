using ComicShop.Domain.Features.Users;
using ComicShop.Infra.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComicShop.Infra.Data.Features.Users
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(255);
            builder.Property(p => p.Email).IsRequired().HasMaxLength(255);
            builder.Property(p => p.Type).IsRequired();
            builder.Property(p => p.Password).IsRequired().HasConversion(passwordToEncrypt => EncryptionHelper.Encrypt(passwordToEncrypt), 
                passwordToDecrypt => EncryptionHelper.Decrypt(passwordToDecrypt));
        }
    }
}
