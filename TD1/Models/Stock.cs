namespace TD1.Models;

public class Stock
{
    // Foreign key for Product
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int QuantityInStock { get; set; }
}
