using System;
using System.Collections.Generic;

namespace TD3.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public partial class Order
{
    [Key]
    public int OrderId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = null!;

    [ForeignKey("Client")]
    public int ClientId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
}
