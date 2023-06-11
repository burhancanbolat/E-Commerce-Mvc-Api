using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulsarECommerceData;
using PulsarECommerceWeb.Areas.Admin.Models;
using System.Data;
using System.Security.Claims;

namespace PulsarECommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrators,ProductAdministrators")]
    public class CategoriesController : Controller
    {
        private readonly AppDbContext context;
        private readonly string entityName = "Kategori";
        public CategoriesController(
            AppDbContext context
            )
        {
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = await context.Categories.Where(p => p.ParentId == null).ToListAsync();
            return View();
        }


        public async Task<IActionResult> TableData(Guid? id, DataTableParameters parameters)
        {
            var query = context.Categories.Where(p => p.ParentId == id);

            var result = new DataTableResult
            {
                data = await query.Skip(parameters.Start).Take(parameters.Length).Select(p => new { p.Name, p.Id }).ToListAsync(),
                draw = parameters.Draw,
                recordsFiltered = await query.CountAsync(),
                recordsTotal = await query.CountAsync()
            };

            return Json(result);
        }

        public async Task<IActionResult> Create(Guid? id)
        {
            ViewBag.Parent = await context.Categories.FindAsync(id);
            return View(new Category { Name = "", Enabled = true, ParentId = id });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category model)
        {

            if(await context.Categories.AnyAsync(p=>p.ParentId == null && p.Name == model.Name))
            {
                TempData["error"] = "Aynı isimli bir başka kayıt olduğundan kayıt işlemi tamamlanamıyor!";
                return View(model);
            }

            model.Id = Guid.NewGuid();
            model.UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            model.DateCreated = DateTime.UtcNow;
            model.Enabled = true;

            context.Categories.Add(model);
            try
            {
                await context.SaveChangesAsync();
                TempData["success"] = $"{entityName} ekleme işlemi başarıyla tamamlanmıştır";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["error"] = "Aynı isimli bir başka kayıt olduğundan kayıt işlemi tamamlanamıyor!";
                return View(model);
            }
        }



        public async Task<IActionResult> Edit(Guid? id)
        {
            var model =  await context.Categories.FindAsync(id);
            ViewBag.Parent = model.Parent;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category model)
        {

            var item = await context.Categories.FindAsync(model.Id);
            item.Name = model.Name;
            item.Enabled = model.Enabled;   
            
            context.Categories.Update(item);
            try
            {
                await context.SaveChangesAsync();
                TempData["success"] = $"{entityName} ekleme işlemi başarıyla tamamlanmıştır";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["error"] = "Aynı isimli bir başka kayıt olduğundan kayıt işlemi tamamlanamıyor!";
                return View(model);
            }
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await context.Categories.FindAsync(id);

            context.Categories.Remove(model!);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                TempData["error"] = $"{entityName} bir yada daha fazla kayıt ile ilişkili olduğundan silinemiyor";
                RedirectToAction(nameof(Index));
            }
            TempData["success"] = $"{entityName} silme işlemi başarıyla tamamlanmıştır";
            return RedirectToAction(nameof(Index));
        }
    }
}
