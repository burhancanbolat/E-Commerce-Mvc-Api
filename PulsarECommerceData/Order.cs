using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulsarECommerceData;

public enum OrderStatus
{
    New, Cancelled, Void, Completed
}

public class Order : BaseEntity
{
    public OrderStatus Status { get; set; }

    public Guid DeliveryAddressId { get; set; }

    public Guid InvoiceAddressId { get; set; }

    public string? TrackingId { get; set; }

    [NotMapped]
    public decimal GrandTotal => OrderDetails.Sum(p => p.LineTotal);

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();

    public virtual DeliveryAddress? DeliveryAddress { get; set; }
    public virtual InvoiceAddress? InvoiceAddress { get; set; }


}

public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder
          .HasMany(p => p.OrderDetails)
          .WithOne(p => p.Order)
          .HasForeignKey(p => p.OrderId)
          .OnDelete(DeleteBehavior.Cascade);
    }
}



