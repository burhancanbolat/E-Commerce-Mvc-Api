using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PulsarECommerceData;
using PulsarECommerceWeb.Areas.Admin.Models;
using System.Security.Claims;

namespace PulsarECommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrators,ProductAdministrators")]
    public class FeaturesController : Controller
    {
        private readonly string entityName = "Özellik";

        private readonly AppDbContext context;
        private readonly IConfiguration configuration;

        public FeaturesController(
            AppDbContext context,
            IConfiguration configuration
            )
        {
            this.context = context;
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> TableData(Guid? id, DataTableParameters parameters)
        {
            var query = context.Features;

            var result = new DataTableResult
            {
                data = await query
                    .Skip(parameters.Start)
                    .Take(parameters.Length)
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        UserName = p.CreatorUser!.Name,
                        CategoryName = p.Category!.Name,
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
            return View(new Feature { Name = "", Enabled = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Feature model)
        {
            model.DateCreated = DateTime.UtcNow;
            model.UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            context.Features.Add(model);
            await context.SaveChangesAsync();
            TempData["success"] = $"{entityName} ekleme işlemi başarıyla tamamlanmıştır";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            ViewBag.Categories = new SelectList((await context.Categories.ToListAsync()).Select(p => new { p.Id, p.PathName }).OrderBy(p => p.PathName), "Id", "PathName");
            var model = await context.Features.FindAsync(id);
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Feature model)
        {

            model.DateCreated = DateTime.UtcNow;
            model.UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            context.Features.Update(model);
            await context.SaveChangesAsync();
            TempData["success"] = $"{entityName} güncelleme işlemi başarıyla tamamlanmıştır";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await context.Features.FindAsync(id);

            context.Features.Remove(model!);
            await context.SaveChangesAsync();

            TempData["success"] = $"{entityName} silme işlemi başarıyla tamamlanmıştır";
            return 
                RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> GetFeaturesByCategoryId(Guid id)
        {
            return Json(await context.Features.Where(p=>p.CategoryId == id).Select(p=> new { p.Id, p.Name }).ToListAsync());
        }

    }
}
