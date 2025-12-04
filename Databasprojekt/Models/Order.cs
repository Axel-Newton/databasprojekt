using System.ComponentModel.DataAnnotations;

namespace Databasprojekt.Models;

public class Order
{
    //PK
    public int OrderId { get; set; }
    
    //FK
    public int CustomerId { get; set; }
    
    [Required]
    public DateTime OrderDate { get; set; }
    
    [Required]
    public decimal TotalAmount { get; set; }
    
    [Required,  MaxLength(100)]
    public string? Status { get; set; }
    
    public OrderRow? OrderRows { get; set; }
}