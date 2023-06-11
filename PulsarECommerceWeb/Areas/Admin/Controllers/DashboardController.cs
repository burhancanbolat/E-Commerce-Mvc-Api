using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulsarECommerceData;

namespace PulsarECommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrators,ProductAdministrators,OrderAdministrators")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext context;

        public DashboardController(
            AppDbContext context
            )
        {
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Comments = await context.Comments.Where(p => !p.Enabled).ToListAsync();
            ViewBag.NewOrders = await context.Orders.CountAsync(p => p.Status == OrderStatus.New);
            return View();
        }

        public async Task<IActionResult> EnableComment(Guid id)
        {
            var model = await context.Comments.FindAsync(id);
            model.Enabled = true;
            context.Comments.Update(model);
            await context.SaveChangesAsync();
            TempData["success"] = "Yorum onaylandı";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var model = await context.Comments.FindAsync(id);
            model.Enabled = true;
            context.Comments.Remove(model);
            await context.SaveChangesAsync();
            TempData["success"] = "Yorum silindi";
            return RedirectToAction(nameof(Index));
        }


    }
}
