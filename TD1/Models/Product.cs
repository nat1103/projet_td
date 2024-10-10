namespace TD1.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

    // Many-to-Many relationship with Supplier
    public ICollection<ProductSupplier> ProductSuppliers { get; set; }

    // One-to-Many relationship with OrderLine
    public ICollection<OrderLine> OrderLines { get; set; }
}