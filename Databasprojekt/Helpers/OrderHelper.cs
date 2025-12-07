using Microsoft.EntityFrameworkCore;

namespace Databasprojekt.Helpers;

public class OrderHelper
{
    static async Task ListOrderSummaryAsync()
    {
        using var db = new ShopContext();

        var summaries = await db.OrderSummaries.OrderByDescending(o => o.OrderDate).ToListAsync();
        
        Console.WriteLine("OrderId | OrderDate | TotalAmount | Customer Email");
        foreach (var summary in summaries)
        {
            Console.WriteLine($"{summary.OrderId} | {summary.OrderDate:D} | {summary.TotalAmount:C} | {summary.CustomerEmail}");
        }
    }
    
    static async Task ListOrdersAsync()
    {
        using var db = new ShopContext(); 
        
        var orders = await db.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.OrderRows)
            .OrderBy(o => o.OrderId)
            .ToListAsync();

        foreach (var order in orders)
        {
            Console.WriteLine($"{order.OrderId} | {order.Customer?.Email} | {order.TotalAmount:C}");
        }
    }
}