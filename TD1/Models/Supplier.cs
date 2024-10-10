namespace TD1.Models;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; }

    // Many-to-Many relationship
    public ICollection<ProductSupplier> ProductSuppliers { get; set; }

}