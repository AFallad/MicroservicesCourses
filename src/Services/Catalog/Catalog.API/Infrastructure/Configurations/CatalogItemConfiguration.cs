using Catalog.API.Models.DomainModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.API.Infrastructure.Configurations
{
    public class CatalogItemConfiguration : IEntityTypeConfiguration<CatalogItem>
    {
        public void Configure(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.Property(ci => ci.Id)
                .UseHiLo()
                .IsRequired();

            builder.Property(ci => ci.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ci => ci.Price)
                .IsRequired();

            builder.HasOne(ci => ci.Brand)
                .WithMany()
                .HasForeignKey(ci => ci.BrandId);

            builder.HasOne(ci => ci.Type)
                .WithMany()
                .HasForeignKey(ci => ci.TypeId);
        }
    }
}
