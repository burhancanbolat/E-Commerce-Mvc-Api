using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulsarECommerceData;

public class Campaign : BaseEntity
{
    [Display(Name = "Açıklama")]
    public required string Descriptions { get; set; }
    public required string Url { get; set; }
    public required string Image { get; set; }

    [Display(Name = "İlk Tarih")]
    [DataType(DataType.Date)]
    public DateTime? DateStart { get; set; }

    [Display(Name = "Son Tarih")]
    [DataType(DataType.Date)] 
    public DateTime? DateEnd { get; set; }

    public Guid CampaignGroupId { get; set; }

    public virtual CampaignGroup? CampaignGroup { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; }


}

public class CampaignEntityTypeConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder
            .Property(p => p.Image)
            .IsUnicode(false);
        builder
            .Property(p => p.Url)
            .HasMaxLength(2048)
            .IsUnicode(false);

    }
}




