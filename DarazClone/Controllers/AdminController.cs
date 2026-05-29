using DarazClone.Data;
using DarazClone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DarazClone.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public AdminController(ApplicationDbContext db,
                               IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // Product List
        public async Task<IActionResult> Index()
        {
            var products = await _db.Products
                .Include(p => p.Category)
                .ToListAsync();
            return View(products);
        }

        // Add Product Form
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View();
        }

        // Save New Product
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            // Handle image upload
            if (product.ImageFile != null &&
                product.ImageFile.Length > 0)
            {
                product.ImageUrl = await SaveImage(product.ImageFile);
            }

            if (ModelState.IsValid)
            {
                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View(product);
        }

        // Edit Product Form
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View(product);
        }

        // Save Edited Product
        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            // Handle image upload
            if (product.ImageFile != null &&
                product.ImageFile.Length > 0)
            {
                product.ImageUrl = await SaveImage(product.ImageFile);
            }

            if (ModelState.IsValid)
            {
                _db.Products.Update(product);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View(product);
        }

        // Delete Product
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product != null)
            {
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // Save image to wwwroot/images/products/
        private async Task<string> SaveImage(IFormFile imageFile)
        {
            string uploadsFolder = Path.Combine(
                _env.WebRootPath, "images", "products");

            // Create folder if it does not exist
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generate unique filename
            string uniqueFileName = Guid.NewGuid().ToString()
                + Path.GetExtension(imageFile.FileName);

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return "/images/products/" + uniqueFileName;
        }
    }
    }