using System.ComponentModel.DataAnnotations.Schema;

namespace PulsarECommerceData;

public class ShoppingCartItem : BaseEntity
{
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual Product? Product { get; set; }

    [NotMapped]
    public decimal Amount => Product!.DiscountedPrice * Quantity;
}
