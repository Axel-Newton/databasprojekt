using System.ComponentModel.DataAnnotations;

namespace Databasprojekt.Models;

public class Category
{
    //PK
    public int CategoryId { get; set; }
    
    [Required,  MaxLength(100)]
    public string? CategoryName { get; set; }
    
    [MaxLength(250)]
    public string? Description { get; set; }
    
    public List<Product> Products { get; set; }
}