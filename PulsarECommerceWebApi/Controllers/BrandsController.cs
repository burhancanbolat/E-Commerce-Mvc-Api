using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulsarECommerceData;
using PulsarECommerceWebApi.Models;

namespace PulsarECommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly AppDbContext context;

        public BrandsController(
            AppDbContext context
            )
        {
            this.context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await 
                context
                .Brands
                .Where(p => p.Enabled)
                .Select(p => new { p.Id, p.Name, p.Logo })
                .OrderBy(p => p.Name)
                .ToListAsync());
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(await context.Brands.SingleOrDefaultAsync(p => p.Id == id));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] BrandViewModel model)
        {
            using var ms = new MemoryStream();
            model.Logo.OpenReadStream().CopyTo(ms);
            var brand = new Brand
            {
                Name = model.Name,
                UserId = Guid.Parse("1d976d27-4ecf-4211-ad4d-70d9d09704b0"),
                DateCreated = DateTime.UtcNow,
                Enabled = true,
                Logo = "data:image/png;base64, " + Convert.ToBase64String(ms.ToArray())

            };
            await context.Brands.AddAsync(brand);
            await context.SaveChangesAsync();
            return Ok(brand);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Put(Guid id, [FromForm] BrandViewModel model)
        {
            var brand = await context.Brands.FindAsync(id);

            brand.Name = model.Name;
            using var ms = new MemoryStream();
            model.Logo.OpenReadStream().CopyTo(ms);
            brand.Logo = "data:image/png;base64, " + Convert.ToBase64String(ms.ToArray());

            context.Brands.Update(brand);
            await context.SaveChangesAsync();
            return Ok(brand);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await context.Brands.FindAsync(id);
            if (model is null)
                return BadRequest("Bulunamadı!");

            context.Brands.Remove(model);
            await context.SaveChangesAsync();
            return Ok(id);
        }


    }
}
