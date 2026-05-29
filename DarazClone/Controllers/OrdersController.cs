using DarazClone.Data;
using DarazClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace DarazClone.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrdersController(ApplicationDbContext db)
        {
            _db = db;
        }

        private List<CartItem> GetCart()
        {
            var json = HttpContext.Session.GetString("Cart");
            return json == null
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(json)!;
        }

        // Show checkout form
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (cart.Count == 0)
                return RedirectToAction("Index", "Cart");

            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(x => x.Total);
            return View();
        }

        // Place order
        [HttpPost]
        public async Task<IActionResult> Checkout(string shippingAddress)
        {
            var cart = GetCart();
            if (cart.Count == 0)
                return RedirectToAction("Index", "Cart");

            var order = new Order
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? "guest",
                ShippingAddress = shippingAddress,
                TotalAmount = cart.Sum(x => x.Total),
                Status = "Pending",
                OrderDate = DateTime.UtcNow,
                Items = cart.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.Price
                }).ToList()
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // Clear cart after order
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Confirmation",
                                    new { id = order.Id });
        }

        // Order confirmation page
        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}