using Microsoft.EntityFrameworkCore;
namespace Databasprojekt.Methods;

public class ListOrderSummary
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
}