using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Azure;
using PulsarECommerceData;
using PulsarECommerceWeb.Areas.Admin.Models;
using PulsarECommerceWeb.Services;
using SixLabors.ImageSharp.Formats.Webp;
using System.Globalization;
using System.Security.Claims;

namespace PulsarECommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrators,ProductAdministrators")]
    public class ProductsController : Controller
    {
        private readonly string entityName = "Ürün";

        private readonly AppDbContext context;
        private readonly IConfiguration configuration;
        private readonly IImageService imageService;

        public ProductsController(
            AppDbContext context,
            IConfiguration configuration,
            IImageService imageService
            )
        {
            this.context = context;
            this.configuration = configuration;
            this.imageService = imageService;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> TableData(Guid? id, DataTableParameters parameters)
        {
            var query = context.Products;

            var result = new DataTableResult
            {
                data = await query
                    .Skip(parameters.Start)
                    .Take(parameters.Length)
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Photo,
                        p.Price,
                        p.DiscountedPrice,
                        p.DiscountRate,
                        UserName = p.CreatorUser!.Name,
                        p.Enabled,
                        DateCreated = p.DateCreated.ToLocalTime().ToShortDateString(),
                    }).ToListAsync(),
                draw = parameters.Draw,
                recordsFiltered = await query.CountAsync(),
                recordsTotal = await query.CountAsync()
            };

            return Json(result);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList((await context.Categories.ToListAsync()).Select(p => new { p.Id, p.PathName }).OrderBy(p => p.PathName), "Id", "PathName");
            ViewBag.Brands = new SelectList(await context.Brands.OrderBy(p => p.Name).Select(p => new { p.Id, p.Name }).ToListAsync(), "Id", "Name");
            return View(new Product { Name = "", PriceText = "0,00", Enabled = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product model)
        {


            if (model.PhotoFile is not null)
                model.Photo = await imageService.GetImageString(model.PhotoFile!);

            if (model.PhotoFiles is not null)
                foreach (var file in model.PhotoFiles)
                    model.ProductPhotos.Add(new ProductPhoto { Photo = await imageService.GetImageString(file) });

            var features = await context.Features.Where(p => p.CategoryId == model.CategoryId).ToListAsync();
            var featureIndex = 0;
            if (model.FeatureValues is not null)
                foreach (var feature in model.FeatureValues)
                    model.ProductFeatures.Add(new ProductFeature { Value = feature, FeatureId = features[featureIndex++].Id });

            model.Price = decimal.Parse(model.PriceText, CultureInfo.CreateSpecificCulture("tr-TR"));
            model.DateCreated = DateTime.UtcNow;
            model.UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            context.Products.Add(model);
            await context.SaveChangesAsync();
            TempData["success"] = $"{entityName} ekleme işlemi başarıyla tamamlanmıştır";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            ViewBag.Categories = new SelectList((await context.Categories.ToListAsync()).Select(p => new { p.Id, p.PathName }).OrderBy(p => p.PathName), "Id", "PathName");
            ViewBag.Brands = new SelectList(await context.Brands.OrderBy(p => p.Name).Select(p => new { p.Id, p.Name }).ToListAsync(), "Id", "Name");
            var model = await context.Products.FindAsync(id);
            model.PriceText = model.Price.ToString("n2", CultureInfo.CreateSpecificCulture("tr-TR"));
            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Edit(Product model)
        {
            if (model.PhotoFile is not null)
                model.Photo = await imageService.GetImageString(model.PhotoFile!);

            if (model.PhotoFiles is not null)
                foreach (var file in model.PhotoFiles)
                    model.ProductPhotos.Add(new ProductPhoto { Photo = await imageService.GetImageString(file) });

            model.DateCreated = DateTime.UtcNow;
            model.UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            model.Price = decimal.Parse(model.PriceText, CultureInfo.CreateSpecificCulture("tr-TR"));
            context.Products.Update(model);
            await context.SaveChangesAsync();
            TempData["success"] = $"{entityName} güncelleme işlemi başarıyla tamamlanmıştır";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await context.Products.FindAsync(id);

            if (model!.Photo is not null)
                _removeFromBlob(model.Photo);

            model.ProductPhotos.ToList().ForEach(p => _removeFromBlob(p.Photo));

            context.Products.Remove(model!);
            await context.SaveChangesAsync();

            TempData["success"] = $"{entityName} silme işlemi başarıyla tamamlanmıştır";
            return RedirectToAction(nameof(Index));

            async void _removeFromBlob(string photo)
            {
                var blobContainer = new BlobContainerClient(
                       configuration.GetValue<string>("Storage:ConnectionString"),
                       configuration.GetValue<string>("Storage:Container")
                       );
                var blobName = photo.Replace($"https://pulsarsecommerce.blob.core.windows.net/pulsarsecommercefiles/", "");

                BlobClient blobClient = blobContainer.GetBlobClient(blobName);
                await blobClient.DeleteAsync();
            }

        }


    }
}
