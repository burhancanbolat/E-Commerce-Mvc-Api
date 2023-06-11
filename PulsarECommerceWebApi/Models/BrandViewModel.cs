namespace PulsarECommerceWebApi.Models
{
    public class BrandViewModel
    {
        public required string Name { get; set; }

        public IFormFile Logo { get; set; }

        public bool Enabled { get; set; }
    }
}
