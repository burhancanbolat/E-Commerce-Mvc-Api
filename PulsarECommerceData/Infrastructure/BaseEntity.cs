using PulsarECommerceData;
using System.ComponentModel.DataAnnotations;

public abstract class BaseEntity
{
    public virtual Guid Id { get; set; }

    [Display(Name = "Aktif")]
    public bool Enabled { get; set; } = true;

    public virtual DateTime DateCreated { get; set; } = DateTime.UtcNow;

    public virtual Guid UserId { get; set; }

    public virtual AppUser? CreatorUser { get; set; }
}

public abstract  class NamedBaseEntity : BaseEntity
{
    [Display(Name="Ad")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz")]
    [MaxLength(450, ErrorMessage = "{0} alanı en fazla {1} karakter olmalıdır")]
    public virtual required string Name { get; set; }

}