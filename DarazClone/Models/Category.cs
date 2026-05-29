using DarazClone.Models;

namespace DarazClone.Models
{
    public class Category { public int Id { get; set; } public string Name { get; set; } = ""; public string? IconClass { get; set; } public ICollection<Product> Products { get; set; } = new List<Product>(); }
}