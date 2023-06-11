using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulsarECommerceData;

namespace PulsarECommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrators, OrderManager")]
    public class OrdersController : Controller
    {
        private readonly AppDbContext context;

        public OrdersController(
            AppDbContext context
            )
        {
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await context.Orders.OrderBy(p=>p.DateCreated).Where(p=>p.Status == OrderStatus.New).ToListAsync());
        }
    }
}
