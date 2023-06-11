using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulsarECommerceData;

public class Brand : NamedBaseEntity
{
    public string? Logo { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();

    [NotMapped]
    public IFormFile? LogoFile { get; set; }
}

public class BrandEntityTypeConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder
            .HasIndex(p => new { p.Name })
            .IsUnique();

        builder
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(450);

        builder
            .Property(p => p.Logo)
            .IsUnicode(false);

        builder
            .HasMany(p => p.Products)
            .WithOne(p => p.Brand)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.SetNull);


    }
}




