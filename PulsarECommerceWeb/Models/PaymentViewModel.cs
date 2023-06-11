namespace PulsarECommerceWeb.Models;

public class PaymentViewModel
{
    public string CardHolderName { get; set; }
    public string CardNumber { get; set; }
    public string Expiry { get; set; }
    public string CVC { get; set; }

    public Guid? DeliveryAddressId { get; set; }
    public Guid? InvoiceAddressId { get; set; }

    public string DeliveryAddressName { get; set; }
    public string DeliveryAddressText { get; set; }
    public string DeliveryAddressDirections { get; set; }

    public string InvoiceAddressName { get; set; }
    public string InvoiceAddressText { get; set; }
    public string InvoiceTaxOffice { get; set; }
    public string InvoiceTaxNumber { get; set; }



}
