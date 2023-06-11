using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulsarECommerceData;

public class Product : NamedBaseEntity
{
    [Display(Name = "Görsel")]
    public string? Photo { get; set; }

    [Display(Name = "Açıklamalar")]
    public string? Descriptions { get; set; }

    public decimal Price { get; set; }

    [Display(Name = "İndirim Oranı")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
    [DataType(DataType.Text)]
    [Range(0,100, ErrorMessage = "{0} alanı en az {1} en çok {2} olmalıdır!")]
    [RegularExpression(@"[0-9]+", ErrorMessage = "Lütfen geçerli bir oran yazınız!")]
    public int DiscountRate { get; set; }

    [Display(Name = "Kategori")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
    public Guid CategoryId { get; set; }

    [Display(Name = "Marka")]
    public Guid? BrandId { get; set; }

    [NotMapped]
    public decimal DiscountedPrice => Price - (Price * DiscountRate / 100.0m);

    [NotMapped]
    public float Rating => Comments.Count > 0 ? (float)Comments.Average(p => p.Rate) : 0;

    [NotMapped]
    public IFormFile? PhotoFile { get; set; }

    [NotMapped]
    public IEnumerable<IFormFile>? PhotoFiles { get; set; }


    [NotMapped]
    public IEnumerable<string>? FeatureValues { get; set; }


    [NotMapped]
    [Display(Name = "Fiyat")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
    [RegularExpression(@"^[0-9.]+(,[0-9]{2})?", ErrorMessage ="Lütfen geçerli bir fiyat yazınız!")]
    public required string PriceText { get; set; }

    public virtual Brand? Brand { get; set; }
    public virtual Category? Category { get; set; }
    public virtual ICollection<ProductPhoto> ProductPhotos { get; set; } = new HashSet<ProductPhoto>();
    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
    public virtual ICollection<ProductFeature> ProductFeatures { get; set; } = new HashSet<ProductFeature>();
    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new HashSet<ShoppingCartItem>();

}

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .HasIndex(p => new { p.Name })
            .IsUnique();

        builder
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(450);

        builder
            .Property(p => p.Photo)
            .IsUnicode(false);

        builder
            .Property(p => p.Price)
            .HasPrecision(18, 4);

        builder
            .HasMany(p => p.ProductPhotos)
            .WithOne(p => p.Product)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(p => p.OrderDetails)
            .WithOne(p => p.Product)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(p => p.Comments)
            .WithOne(p => p.Product)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(p => p.ProductFeatures)
            .WithOne(p => p.Product)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(p => p.ShoppingCartItems)
            .WithOne(p => p.Product)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}




