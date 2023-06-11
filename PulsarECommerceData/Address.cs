using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace PulsarECommerceData;

public abstract class Address : NamedBaseEntity
{
    public string Text { get; set; }
    public string? DisplayName { get; set; }

   
}

public class AddressEntityTypeConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {

    }
}

public class DeliveryAddress : Address
{
    public string? Directions { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();

}
public class DeliveryAddressEntityTypeConfiguration : IEntityTypeConfiguration<DeliveryAddress>
{
    public void Configure(EntityTypeBuilder<DeliveryAddress> builder)
    {
        builder
            .HasMany(p => p.Orders)
            .WithOne(p => p.DeliveryAddress)
            .HasForeignKey(p => p.DeliveryAddressId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
public class InvoiceAddress : Address
{
    public string? TaxOffice { get; set; }
    public string? TaxNumber { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();

}

public class InvoiceAddressEntityTypeConfiguration : IEntityTypeConfiguration<InvoiceAddress>
{
    public void Configure(EntityTypeBuilder<InvoiceAddress> builder)
    {


        builder
            .HasMany(p => p.Orders)
            .WithOne(p => p.InvoiceAddress)
            .HasForeignKey(p => p.InvoiceAddressId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}