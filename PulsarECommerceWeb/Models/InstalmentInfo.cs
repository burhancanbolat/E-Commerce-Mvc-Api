namespace PulsarECommerceWeb.Models
{
    public class InstalmentInfo
    {
        public string LogoUrl { get; set; }
        public List<InstalmentInfoDetail> Installments { get; set; } = new List<InstalmentInfoDetail>();
    }

    public class InstalmentInfoDetail
    {
        public int Instalment { get; set; }
        public decimal Rate { get; set; }
        
    }
}
