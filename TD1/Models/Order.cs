namespace TD1.Models;

public class Order
{
    public int OrderId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; }

    // Foreign key for Client
    public int ClientId { get; set; }
    public Client Client { get; set; }

    // One-to-Many relationship with OrderLine
    public ICollection<OrderLine> OrderLines { get; set; }
}
