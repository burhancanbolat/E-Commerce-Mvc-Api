using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulsarECommerceData;

public class Category : NamedBaseEntity
{
    public Guid? ParentId { get; set; }

    public virtual Category? Parent { get; set; }
    public virtual ICollection<Category> Children { get; set; } = new HashSet<Category>();
    public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    public virtual ICollection<Feature> Features { get; set; } = new HashSet<Feature>();



    [NotMapped]
    public List<Category> Path
    {
        get
        {
            var list = new List<Category>();
            _populate(ref list, this);
            list.Reverse();
            return list;

            List<Category> _populate(ref List<Category> list, Category item)
            {
                if (item is null)
                    return list;
                list.Add(item);
                if (item.ParentId is not null)
                    _populate(ref list, item.Parent!);
                return list;
            }
        }
    }

    [NotMapped]
    public string PathName => string.Join(" / ", Path.Select(p => p.Name));

}

public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder
            .HasIndex(p => new { p.ParentId, p.Name })
            .IsUnique();

        builder
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(450);

        builder
            .HasMany(p => p.Children)
            .WithOne(p => p.Parent)
            .HasForeignKey(p => p.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(p => p.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(p => p.Features)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

//Id        Name            ParentId        
// 1        Moda            null
// 2        Elektronik      null
// 3        Ev, Yaşam       null
// 4        Kadın           1
// 5        Erkek           1
// 6        Giyim           4
// 7        Giyim           5
// 8        Sweatshirt      7
// 9        Elbise          6