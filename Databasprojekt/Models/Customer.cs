using System.ComponentModel.DataAnnotations;

namespace Databasprojekt.Models;

public class Customer
{
    //PK
    public int CustomerId { get; set; }
    
    [Required, MaxLength(100)]
    public string? Name { get; set; }
    
    [Required, MaxLength(100)]
    public string? Email { get; set; }
    
    //Optional
    [MaxLength(100)]
    public string? City  { get; set; }

    public List<Order> Orders { get; set; } = new();
}