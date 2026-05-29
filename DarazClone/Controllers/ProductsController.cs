using DarazClone.Data;
using DarazClone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DarazClone.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(
            string? search,
            int? categoryId,
            string sort = "newest")
        {
            var q = _db.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                q = q.Where(p => p.Name.Contains(search));

            if (categoryId.HasValue)
                q = q.Where(p => p.CategoryId == categoryId);

            q = sort switch
            {
                "price-asc" => q.OrderBy(p => p.Price),
                "price-desc" => q.OrderByDescending(p => p.Price),
                _ => q.OrderByDescending(p => p.CreatedAt)
            };

            ViewBag.Categories = await _db.Categories.ToListAsync();

            return View(await q.ToListAsync());
        }

        public async Task<IActionResult> Detail(int id)
        {
            var p = await _db.Products
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null)
                return NotFound();

            return View(p);
        }
    }
}