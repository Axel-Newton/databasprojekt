using System.ComponentModel.DataAnnotations;

namespace Databasprojekt.Models;

public class Product
{
    //PK
    public int ProductId { get; set; }
    
    [Required,  MaxLength(100)]
    public string? Name { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    [MaxLength(250)]
    public string? Description { get; set; }
    
    public Category? Category { get; set; }
    
    //FK
    public int CategoryId { get; set; }
}