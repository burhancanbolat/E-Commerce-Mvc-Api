using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PulsarECommerceData;

public class Comment : BaseEntity
{

    public string? Text { get; set; }

    public int Rate { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    public Guid ProductId { get; set; }

    public virtual Product? Product { get; set; }

}

public class CommentEntityTypeConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        

    }
}




