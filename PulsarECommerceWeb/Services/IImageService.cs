using Azure.Storage.Blobs;
using SixLabors.ImageSharp.Formats.Webp;

namespace PulsarECommerceWeb.Services;

public interface IImageService
{
    
    Task<string> GetImageString(IFormFile file, int width = 800, int height = 600);
}

public class ImageService : IImageService
{
    private readonly IConfiguration configuration;

    public ImageService(
        IConfiguration configuration
        )
    {
        this.configuration = configuration;
    }

    public async Task<string> GetImageString(IFormFile file, int width = 800, int height = 600)
    {

        using var image = await Image.LoadAsync(file.OpenReadStream());

        image.Mutate(p => p.Resize(new ResizeOptions
        {
            Size = new Size(width, height),
            Mode = ResizeMode.Crop
        }));
        using var ms = new MemoryStream();

        var useStorage = configuration.GetValue<bool>("UseStorage");
        if (useStorage)
        {
            var blobContainer = new BlobContainerClient(
             configuration.GetValue<string>("Storage:ConnectionString"),
             configuration.GetValue<string>("Storage:Container")
             );
            var blobName = $"product_{Path.GetRandomFileName()}.webp";
            BlobClient blobClient = blobContainer.GetBlobClient(blobName);
            ;
            ms.Seek(0, SeekOrigin.Begin);
            await blobClient.UploadAsync(ms);
            image.SaveAsWebp(ms);
            return $"https://pulsarsecommerce.blob.core.windows.net/pulsarsecommercefiles/{blobName}";
        }
        else
        {
            return image.ToBase64String(WebpFormat.Instance);
        }
    }
}