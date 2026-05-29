using DarazClone.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Order
{
    public int Id { get; set; }

    public string UserId { get; set; } = "";

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "Pending";

    public string ShippingAddress { get; set; } = "";

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}