using CatalogService.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Persistence.Configurations;

public sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Url).HasMaxLength(500).IsRequired();
        builder.Property(x => x.AltText).HasMaxLength(180).IsRequired();
        builder.Property(x => x.SortOrder).IsRequired();
        builder.Property(x => x.IsPrimary).IsRequired();
        builder.HasIndex(x => x.ProductId);
    }
}
