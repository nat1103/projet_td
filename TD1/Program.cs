// See https://aka.ms/new-console-template for more information

using TD1.Data;
using TD1.Models;

using (var ctx = new ElectroShopContext()) // using permet de fermer la connexion à la db
{
    
    Product prod = new Product();
    prod.Name = "Laptop";
    prod.Price = 1000;
    prod.Stock = 10;
    ctx.Products.Add(prod);
    Supplier sup = new Supplier();
    sup.Name = "Dell";
    sup.Contact = "noreply@dell.fr";
    ctx.Suppliers.Add(sup);
    ProductSupplier ps = new ProductSupplier(); 
    ps.Product = prod;
    ps.Supplier = sup;
    ctx.ProductSuppliers.Add(ps);
    Product prod2 = new Product();
    prod2.Name = "Desktop";
    prod2.Price = 800;
    prod2.Stock = 5;
    ctx.Products.Add(prod2);
    ProductSupplier ps2 = new ProductSupplier(); 
    ps2.Product = prod2;
    ps2.Supplier = sup;
    ctx.ProductSuppliers.Add(ps2);
    ctx.SaveChanges();

    foreach (var product in ctx.Products)
    {
        Console.WriteLine(product);
        if (product.ProductSuppliers != null)
        {
            foreach (var productSuppliers in product.ProductSuppliers)
            {
                Console.WriteLine(productSuppliers.Supplier);
            }
        }
    }
}