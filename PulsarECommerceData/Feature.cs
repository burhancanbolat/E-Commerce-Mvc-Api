using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace PulsarECommerceData;

public class Feature : NamedBaseEntity
{
    [Display(Name = "Kategori")]
    [Required(ErrorMessage ="{0} alanı boş bırakılamaz!")]
    public Guid CategoryId { get; set; }

    public virtual Category? Category { get; set; }
    public virtual ICollection<ProductFeature> ProductFeatures { get; set; } = new HashSet<ProductFeature>();

}

public class FeatureEntityTypeConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder
            .HasIndex(p => new { p.Name, p.CategoryId })
            .IsUnique();

        builder
            .HasMany(p => p.ProductFeatures)
            .WithOne(p => p.Feature)
            .HasForeignKey(p => p.FeatureId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}






