using Microsoft.EntityFrameworkCore;

namespace Databasprojekt.Methods;

public class ListOrders
{
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