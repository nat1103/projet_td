using System;
using System.Collections.Generic;

namespace TD3.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime Date { get; set; }

    public string Status { get; set; } = null!;

    public int ClientId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
}
