using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PulsarECommerceData;
using PulsarECommerceWeb.Areas.Admin.Models;
using PulsarECommerceWeb.Services;
using System.Security.Claims;

namespace PulsarECommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrators,ProductAdministrators")]
    public class CampaignsController : Controller
    {
        private readonly string entityName = "Kampanya";

        private readonly AppDbContext context;
        private readonly IConfiguration configuration;
        private readonly IImageService imageService;

        public CampaignsController(
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
            var query = context.Campaigns;

            var result = new DataTableResult
            {
                data = await query
                    .Skip(parameters.Start)
                    .Take(parameters.Length)
                    .Select(p => new
                    {
                        p.Id,
                        p.Image,
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
            ViewBag.CampaignGroups = new SelectList(await context.CampaignGroups.ToListAsync(), "Id", "Name");
            return View(new Campaign { Descriptions = "", Image = default, Url = "", Enabled = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Campaign model)
        {
            if (model.ImageFile is not null)
                model.Image = await imageService.GetImageString(model.ImageFile, 1920, 1080);

            model.DateCreated = DateTime.UtcNow;
            model.UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            context.Campaigns.Add(model);
            await context.SaveChangesAsync();
            TempData["success"] = $"{entityName} ekleme işlemi başarıyla tamamlanmıştır";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            ViewBag.CampaignGroups = new SelectList(await context.CampaignGroups.ToListAsync(), "Id", "Name");
            var model = await context.Campaigns.FindAsync(id);
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Campaign model)
        {

            if (model.ImageFile is not null)
                model.Image = await imageService.GetImageString(model.ImageFile, 1920,1080);

            model.DateCreated = DateTime.UtcNow;
            model.UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            context.Campaigns.Update(model);
            await context.SaveChangesAsync();
            TempData["success"] = $"{entityName} güncelleme işlemi başarıyla tamamlanmıştır";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await context.Campaigns.FindAsync(id);

            if (model!.Image is not null)
            {
                var blobContainer = new BlobContainerClient(
                       configuration.GetValue<string>("Storage:ConnectionString"),
                       configuration.GetValue<string>("Storage:Container")
                       );
                var blobName = model.Image.Replace($"https://pulsarsecommerce.blob.core.windows.net/pulsarsecommercefiles/", "");

                BlobClient blobClient = blobContainer.GetBlobClient(blobName);
                await blobClient.DeleteAsync();
            }

            context.Campaigns.Remove(model!);
            await context.SaveChangesAsync();

            TempData["success"] = $"{entityName} silme işlemi başarıyla tamamlanmıştır";
            return RedirectToAction(nameof(Index));
        }


    }
}
