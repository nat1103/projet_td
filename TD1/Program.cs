// See https://aka.ms/new-console-template for more information

using TD1.Data;
using TD1.Models;

using (var ctx = new ElectroShopContext()) // using permet de fermer la connexion à la db
{
    
    ProductSupplier ps = new ProductSupplier(); 
    
    Product prod = new Product();
    prod.Name = "Laptop";
    prod.Price = 1000;
    prod.Stock = 10;
    ctx.Products.Add(prod);
    Supplier sup = new Supplier();
    sup.Name = "Dell";
    sup.Contact = "noreply@dell.fr";
    ctx.Suppliers.Add(sup);
    ps.Product = prod;
    ps.Supplier = sup;
    ctx.ProductSuppliers.Add(ps);
    Product prod2 = new Product();
    prod2.Name = "Desktop";
    prod2.Price = 800;
    prod2.Stock = 5;
    ctx.Products.Add(prod2);
    ps.Product = prod2;
    ps.Supplier = sup;
    ctx.ProductSuppliers.Add(ps);
    ctx.SaveChanges();

    foreach (var product in ctx.Products)
    {
        Console.WriteLine(product.Name);
        if(product.ProductSuppliers != null)
        {
            foreach (var productSuppliers in product.ProductSuppliers)
            {
                Console.WriteLine($"\t{productSuppliers.Supplier.Name}");
            }
        }
    }
}