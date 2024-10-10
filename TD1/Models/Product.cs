namespace TD1.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }

    // Relation Many-to-Many
    public ICollection<ProductSupplier> ProductSuppliers { get; set; }

}