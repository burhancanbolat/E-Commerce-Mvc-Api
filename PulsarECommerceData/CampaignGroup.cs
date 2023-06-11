using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulsarECommerceData;

public class CampaignGroup : NamedBaseEntity
{
    public string? BackgroundImage { get; set; }
    public virtual ICollection<Campaign> Campaigns { get; set; } = new HashSet<Campaign>();

    [NotMapped]
    public IFormFile? BackgroundImageFile { get; set; }

}

public class CampaignGroupEntityTypeConfiguration : IEntityTypeConfiguration<CampaignGroup>
{
    public void Configure(EntityTypeBuilder<CampaignGroup> builder)
    {

        builder
            .Property(p => p.BackgroundImage)
            .IsUnicode(false);
        builder
            .HasMany(p => p.Campaigns)
            .WithOne(p => p.CampaignGroup)
            .HasForeignKey(p => p.CampaignGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}




