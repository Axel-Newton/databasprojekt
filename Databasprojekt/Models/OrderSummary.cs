using Microsoft.EntityFrameworkCore;

namespace Databasprojekt.Models;

/* TA BORT DESSA KOMMENTARER INNAN INLÄMNING
 * Keyless entity (no PK)
 * Den representerar en SQL view, en spara SELECT-query
 * Vi använder dessa Views i EF Core som gör att den kan läsa den precis som en vanlig tabell
 */

[Keyless] //Optional
public class OrderSummary
{
    public int OrderId { get; set; }
    
    public DateTime OrderDate { get; set; }
    
    public string CustomerName { get; set; } = string.Empty; //Always has a value
    
    public string CustomerEmail { get; set; } = string.Empty;
    
    public decimal TotalAmount { get; set; }
}