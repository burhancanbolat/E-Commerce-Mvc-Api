using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PulsarECommerceData;

public enum Genders
{
    [Display(Name = "Erkek")]
    Male,
    [Display(Name = "Kadın")]
    Female
}

public class AppUser : IdentityUser<Guid>
{

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public Genders? Gender { get; set; }
    public bool Enabled { get; set; } = true;

    [NotMapped]
    public string Name => $"{FirstName} {LastName}";

    public virtual ICollection<Category> CreatedCategories { get; set; } = new HashSet<Category>();
    public virtual ICollection<Brand> CreatedBrands { get; set; } = new HashSet<Brand>();
    public virtual ICollection<Product> CreatedProducts { get; set; } = new HashSet<Product>();
    public virtual ICollection<Campaign> CreatedCampaigns { get; set; } = new HashSet<Campaign>();
    public virtual ICollection<CampaignGroup> CreatedCampaignGroups { get; set; } = new HashSet<CampaignGroup>();
    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public virtual ICollection<Feature> CreatedFeatures { get; set; } = new HashSet<Feature>();
    public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new HashSet<ShoppingCartItem>();
    public virtual ICollection<Address> Addresses { get; set; } = new HashSet<Address>();

}

public class AppUserEntityTypeConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder
            .HasIndex(p => new { p.FirstName, p.LastName })
            .IsUnique(false);

        builder
            .Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(450);

        builder
            .Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(450);

        builder
            .HasMany(p => p.CreatedCategories)
            .WithOne(p => p.CreatorUser)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(p => p.CreatedBrands)
            .WithOne(p => p.CreatorUser)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(p => p.CreatedProducts)
            .WithOne(p => p.CreatorUser)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(p => p.CreatedCampaigns)
            .WithOne(p => p.CreatorUser)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(p => p.CreatedCampaignGroups)
            .WithOne(p => p.CreatorUser)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasMany(p => p.Comments)
            .WithOne(p => p.CreatorUser)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(p => p.CreatedFeatures)
            .WithOne(p => p.CreatorUser)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(p => p.Orders)
            .WithOne(p => p.CreatorUser)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(p => p.ShoppingCartItems)
            .WithOne(p => p.CreatorUser)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(p => p.Addresses)
            .WithOne(p => p.CreatorUser)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}

