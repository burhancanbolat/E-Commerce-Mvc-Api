﻿using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class CampaignGroupsController : Controller
    {
        private readonly string entityName = "Kampanya Grubu";

        private readonly AppDbContext context;
        private readonly IConfiguration configuration;
        private readonly IImageService imageService;

        public CampaignGroupsController(
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
            var query = context.CampaignGroups;

            var result = new DataTableResult
            {
                data = await query
                    .Skip(parameters.Start)
                    .Take(parameters.Length)
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.BackgroundImage,
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


        public IActionResult Create()
        {
            return View(new CampaignGroup { Name = "", Enabled = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CampaignGroup model)
        {
            
            if (model.BackgroundImageFile is not null)
                model.BackgroundImage = await imageService.GetImageString(model.BackgroundImageFile, 1920, 1080);

            model.DateCreated = DateTime.UtcNow;
            model.UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            context.CampaignGroups.Add(model);
            await context.SaveChangesAsync();
            TempData["success"] = $"{entityName} ekleme işlemi başarıyla tamamlanmıştır";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await context.CampaignGroups.FindAsync(id);
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(CampaignGroup model)
        {

            if (model.BackgroundImageFile is not null)
                model.BackgroundImage = await imageService.GetImageString(model.BackgroundImageFile, 1920, 1080);

            model.DateCreated = DateTime.UtcNow;
            model.UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            context.CampaignGroups.Update(model);
            await context.SaveChangesAsync();
            TempData["success"] = $"{entityName} güncelleme işlemi başarıyla tamamlanmıştır";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await context.CampaignGroups.FindAsync(id);

            if (model!.BackgroundImage is not null)
            {
                var blobContainer = new BlobContainerClient(
                       configuration.GetValue<string>("Storage:ConnectionString"),
                       configuration.GetValue<string>("Storage:Container")
                       );
                var blobName = model.BackgroundImage.Replace($"https://pulsarsecommerce.blob.core.windows.net/pulsarsecommercefiles/", "");

                BlobClient blobClient = blobContainer.GetBlobClient(blobName);
                await blobClient.DeleteAsync();
            }

            context.CampaignGroups.Remove(model!);
            await context.SaveChangesAsync();

            TempData["success"] = $"{entityName} silme işlemi başarıyla tamamlanmıştır";
            return RedirectToAction(nameof(Index));
        }


    }
}
