using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PulsarECommerceData;

public class ProductPhoto 
{
    public Guid Id { get; set; }

    public string? Photo { get; set; }

    public Guid ProductId { get; set; }

    public virtual Product? Product { get; set; }

}

public class ProductPhotoEntityTypeConfiguration : IEntityTypeConfiguration<ProductPhoto>
{
    public void Configure(EntityTypeBuilder<ProductPhoto> builder)
    {
        builder
            .Property(p => p.Photo)
            .IsUnicode(false);
    }
}




