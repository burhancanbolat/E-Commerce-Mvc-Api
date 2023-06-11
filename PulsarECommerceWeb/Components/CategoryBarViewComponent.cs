using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulsarECommerceData;

namespace PulsarECommerceWeb.Components
{
    public class CategoryBarViewComponent : ViewComponent
    {
        private readonly AppDbContext context;

        public CategoryBarViewComponent(
            AppDbContext context
            )
        {
            this.context = context;
        }

        public IViewComponentResult Invoke()
        {
            var model = context.Categories.Where(p => p.ParentId == null).ToList();
            return View(model);
        }
    }
}
