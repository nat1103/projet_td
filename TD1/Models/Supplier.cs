namespace TD1.Models;

public class Supplier
{
    public int SupplierId { get; set; }
    public string Name { get; set; }
    public string Contact { get; set; }

    // Many-to-Many relationship with Product
    public ICollection<ProductSupplier> ProductSuppliers { get; set; }
    
    public override string ToString()
    {
        return $"Supplier: {Name}, Contact: {Contact}";
    }
}
