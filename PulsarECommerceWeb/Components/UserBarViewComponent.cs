using Microsoft.AspNetCore.Mvc;
using PulsarECommerceData;
using System.Security.Claims;

namespace PulsarECommerceWeb.Components;

public class UserBarViewComponent : ViewComponent
{
    private readonly AppDbContext context;

    public UserBarViewComponent(
        AppDbContext context
        )
    {
        this.context = context;
    }

    public IViewComponentResult Invoke()
    {
        if (User.Identity.IsAuthenticated)
        {
            var userId = Guid.Parse(((ClaimsPrincipal)User).FindFirstValue(ClaimTypes.NameIdentifier));
            var model = context
                .ShoppingCartItems
                .Where(p => p.UserId == userId)
                .ToList();
            return View(model);
        }
        return View();
    }

}
