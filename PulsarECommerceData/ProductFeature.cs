using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PulsarECommerceData;

public class ProductFeature
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid FeatureId { get; set; }

    public required string Value { get; set; }

    public virtual Product? Product { get; set; }
    public virtual Feature? Feature { get; set; }
}

public class ProductFeatureEntityTypeConfiguration : IEntityTypeConfiguration<ProductFeature>
{
    public void Configure(EntityTypeBuilder<ProductFeature> builder)
    {

    }
}






