using System.Diagnostics;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PulsarECommerceData;
using PulsarECommerceWeb.Models;

namespace PulsarECommerceWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext context;

    public HomeController(
        ILogger<HomeController> logger,
        AppDbContext context
        )
    {
        _logger = logger;
        this.context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.CampaignGroups = await context.CampaignGroups.Where(p => p.Enabled && p.Campaigns.Any()).ToListAsync();
        ViewBag.LatestProducts = await context.Products.Where(p => p.Enabled).OrderByDescending(p => p.DateCreated).Take(12).ToListAsync();
        ViewBag.Brands = await context.Brands.Where(p => p.Enabled && p.Products.Any()).OrderBy(p => p.Name).ToListAsync();
        return View();
    }

    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        return LocalRedirect(returnUrl);
    }


    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Category(Guid id, decimal? min, decimal? max)
    {
        var category = await context.Categories.FindAsync(id);
        ViewBag.Category = category;

        var categories = context.Categories.Where(p => p.Id == id).SelectMany(p => p.Children).Select(p => p.Id).ToList();
        categories.Add(id);
        var model = await context
            .Products
            .Where(p => categories.Any(q => q == p.CategoryId) && p.Enabled && (p.Price - (p.Price * p.DiscountRate / 100) >= min || min == null) && (p.Price - (p.Price * p.DiscountRate / 100) <= max || max == null))
            .ToListAsync();

        return View(model);
    }

    public async Task<IActionResult> Brand(Guid id)
    {
        var model = await context.Brands.FindAsync(id);
        return View(model);
    }
    public async Task<IActionResult> Product(Guid id)
    {
        ViewBag.IsProductOrdered = false;
        if (User.Identity.IsAuthenticated)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ViewBag.IsProductOrdered = await context.Orders.AnyAsync(p => p.UserId == userId && p.OrderDetails.Any(q => q.ProductId == id));
        }
        var model = await context.Products.FindAsync(id);
        return View(model);
    }
    public async Task<IActionResult> Search(string keywords)
    {
        var words = Regex.Split(keywords.ToLower(), @"\s+").ToList()!;
        var model = context
            .Products
            .ToList()
            .Where(p =>
            words.All(q => p.Name.ToLower().Contains(q)) ||
            words.All(q => ((p.Brand?.Name) ?? "").ToLower().Contains(q)) ||
            words.All(q => p.Category.Name.Contains(q))
            )
            .ToList();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Comment(CommentViewModel model)
    {
        var comment = new Comment
        {
            Date = DateTime.UtcNow,
            Enabled = false,
            Rate = model.Rate,
            Text = model.Text,
            ProductId = model.ProductId,
            UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
        };

        await context.Comments.AddAsync(comment);
        await context.SaveChangesAsync();
        TempData["success"] = "Yorumunuz tarafımıza ulaşmıştır.";
        return RedirectToAction(nameof(Product), new { id = model.ProductId });

    }

    [Authorize]
    public async Task<IActionResult> AddToCart(Guid id, int? qty)
    {
        var model = await context.ShoppingCartItems.SingleOrDefaultAsync(p => p.ProductId == id);
        if (model == null)
        {
            model = new ShoppingCartItem
            {
                UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                ProductId = id,
                Quantity = qty ?? 1,
                DateCreated = DateTime.UtcNow,
                Enabled = true,
            };
            await context.ShoppingCartItems.AddAsync(model);
        }
        else
        {
            model.Quantity += qty ?? 1;
            context.ShoppingCartItems.Update(model);
        }
        await context.SaveChangesAsync();
        TempData["success"] = "Ürün sepetinize eklenmiştir.";
        return Redirect(Request.Headers["Referer"].ToString());
    }


    [Authorize]
    [HttpGet()]
    public async Task<IActionResult> CheckOut()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(id);
        var model = await context.ShoppingCartItems
            .Where(p => p.UserId == userId)
            .ToListAsync();
        return View(model);
    }


    [Authorize]
    [HttpGet()]
    public async Task<IActionResult> RemoveFromCart(Guid id)
    {
        await context
            .ShoppingCartItems
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync();

        return RedirectToAction(nameof(CheckOut));
    }

    [Authorize]
    [HttpGet()]
    public async Task<IActionResult> UpdateCartItem(Guid id, int qty)
    {
        var model = await context.ShoppingCartItems.SingleOrDefaultAsync(p => p.Id == id);
        model.Quantity = qty;
        context.ShoppingCartItems.Update(model);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(CheckOut));

    }
    [Authorize]
    [HttpGet()]
    public async Task<IActionResult> ClearShoppingCart()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await context
            .ShoppingCartItems
            .Where(p => p.UserId == userId)
            .ExecuteDeleteAsync();
        return RedirectToAction(nameof(Index));

    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Payment()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        ViewBag.ShoppingCartItems = await context.ShoppingCartItems.Where(p => p.UserId == userId).ToListAsync();
        ViewBag.DeliveryAddresses = new SelectList(await context.Addresses.OfType<DeliveryAddress>().Where(p => p.UserId == userId).ToListAsync(), "Id", "Name");
        ViewBag.InvoiceAddresses = new SelectList(await context.Addresses.OfType<InvoiceAddress>().Where(p => p.UserId == userId).ToListAsync(), "Id", "Name");
        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Payment(PaymentViewModel model)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        /*
         * KK tahsilat işlemi burada
         * */
        var deliveryAddress = default(DeliveryAddress);
        if (model.DeliveryAddressId is null)
        {
            deliveryAddress = new DeliveryAddress
            {
                Name = model.DeliveryAddressName,
                UserId = userId,
                DateCreated = DateTime.UtcNow,
                Enabled = true,
                Text = model.DeliveryAddressText,
                Directions = model.DeliveryAddressDirections,

            };
            await context.Addresses.AddAsync(deliveryAddress);
        }
        var invoiceAddress = default(InvoiceAddress);
        if (model.InvoiceAddressId is null)
        {
            invoiceAddress = new InvoiceAddress
            {
                Name = model.InvoiceAddressName,
                UserId = userId,
                DateCreated = DateTime.UtcNow,
                Enabled = true,
                Text = model.InvoiceAddressText,
                TaxOffice = model.InvoiceTaxOffice,
                TaxNumber = model.InvoiceTaxNumber
            };
            await context.Addresses.AddAsync(invoiceAddress);
        }
        var items = await context.ShoppingCartItems.Where(p => p.UserId == userId).ToListAsync();
        var order = new Order
        {
            DateCreated = DateTime.UtcNow,
            Enabled = true,
            Status = OrderStatus.New,
            UserId = userId,
            DeliveryAddressId = model.DeliveryAddressId ?? deliveryAddress!.Id,
            InvoiceAddressId = model.InvoiceAddressId ?? invoiceAddress!.Id,
            OrderDetails = items.Select(p => new OrderDetail
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity,
                DiscountRate = p.Product.DiscountRate,
                Price = p.Product.Price,
            }).ToList()
        };


        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(ClearShoppingCart));

    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
