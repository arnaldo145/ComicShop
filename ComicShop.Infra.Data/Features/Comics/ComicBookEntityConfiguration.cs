using ComicShop.Domain.Comics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComicShop.Infra.Data.Features.Comics
{
    public class ComicBookEntityConfiguration : IEntityTypeConfiguration<ComicBook>
    {
        public void Configure(EntityTypeBuilder<ComicBook> builder)
        {
            builder.ToTable("ComicBooks");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(255);
            builder.Property(c => c.ReleaseYear).IsRequired().HasMaxLength(4);
            builder.Property(c => c.Price).IsRequired();

            builder.HasOne(c => c.Publisher).WithMany(p => p.ComicBooks).HasForeignKey(c => c.PublisherId).IsRequired();
        }
    }
}
