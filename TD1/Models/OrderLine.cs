namespace TD1.Models;

public class OrderLine
{
    public int OrderLineId { get; set; }
    
    // Foreign key for Order
    public int OrderId { get; set; }
    public Order Order { get; set; }

    // Foreign key for Product
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
