using Microsoft.EntityFrameworkCore;
using Databasprojekt.Models;
namespace Databasprojekt.Helpers;

public class OrderHelper
{
    public static async Task OrderMenuAsync()
    {
        using var db = new ShopContext();
        
        Console.WriteLine("What would you like to do?");
        Console.WriteLine("1. Create new order");
        Console.WriteLine("2. List all orders");
        Console.WriteLine("3. OrderSummary");
        Console.WriteLine("4. Delete order");
        Console.WriteLine("");
        
        var choice = Console.ReadKey().KeyChar;

        if (choice == '1')
        {
            await CreateOrderAsync();
        }
        else if (choice == '2')
        {
            await ListOrdersAsync();
        }
        else if (choice == '3')
        {
            await DeleteOrderAsync();
        }
        else if (choice == '4')
        {
            Console.WriteLine("Returning...");
            return;
        }
        else
        {
            Console.WriteLine("Invalid choice.");
        }
        
    }
    static async Task CreateOrderAsync()
    {
        using var db = new ShopContext();
        Console.WriteLine("Enter CustomerId:");
        var customerId = Console.ReadLine();

        if (string.IsNullOrEmpty(customerId))
        {
            Console.WriteLine("Invalid customerId!, can not be empty");
        }
        
        var order = new Order
        {
            CustomerId = Convert.ToInt32(customerId), 
            OrderDate = DateTime.Now,
            TotalAmount = 0m,
            OrderRows = new List<OrderRow>(),
            Status = string.Empty
        };
        db.Orders.Add(order);
        
        while(true)
        {
            await ProductHelper.ListProductsAsync();
            Console.WriteLine("Choose ProductId:");
            if (!int.TryParse(Console.ReadLine(), out var productId))
            {
                Console.WriteLine("Invalid productId!, please enter a valid number");
                return;
            }

            var product = await db.Products.FindAsync(productId);
            {
                if (product == null)
                {
                    Console.WriteLine("ProductId not found!");
                    return;
                }
            }

            Console.WriteLine("How many would you like to order?");
            var quantity = Console.ReadLine();
            if (!int.TryParse(quantity, out var parsedQuantity)  || parsedQuantity > product.StockQuantity)
            {
                Console.WriteLine("Invalid quantity!, can not be empty or more than " + product.StockQuantity);
                return;
            }
            
            var orderrow = new OrderRow
            {
                ProductId = product.ProductId,
                Quantity = Convert.ToInt32(quantity),
                UnitPrice = product.Price,
                Order = order
            };
            order.OrderRows.Add(orderrow);
            await db.OrderRows.AddAsync(orderrow);
            
            product.StockQuantity -= orderrow.Quantity;
            db.Products.Update(product);
            
            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Order added!");
            }
            catch (DbUpdateException exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine("Something went wrong :(");
            }
            Console.WriteLine("Add another product? (y/n)");
            var answer = Console.ReadLine()?.ToLower().Trim();
            if (answer == "y")
            {
                continue;
            }
            break;
            
        }
        Console.WriteLine("Enter Status:");
        var status = Console.ReadLine();
        order.Status = status;
        order.TotalAmount = order.OrderRows.Sum(r => r.Quantity * r.UnitPrice);
        await db.SaveChangesAsync();
    }

    static async Task ListOrderSummaryAsync()
    {
        using var db = new ShopContext();
        
        await AdminHelper.AdminCheckAsync();
        
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
        
        await AdminHelper.AdminCheckAsync();
        
        var sq = System.Diagnostics.Stopwatch.StartNew();
        
        Console.WriteLine("Enter Page:");
        var page = int.Parse(Console.ReadLine().Trim());
        
        Console.WriteLine("Enter PageSize:");
        var pageSize = int.Parse(Console.ReadLine().Trim());
        
        var query = db.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.OrderRows)
            .OrderBy(o => o.OrderDate);
        
        sq.Stop();
        Console.WriteLine($"Total Time: {sq.ElapsedMilliseconds} ms");
        
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        var orders = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();    
        
        Console.WriteLine($"Page: {page}, PageSize: {pageSize}, TotalPages: {totalPages}");
        Console.WriteLine("OrderId | OrderDate | TotalAmount | Customer Email | Status");
        foreach (var order in orders)
        {
            Console.WriteLine($"{order.OrderId} | {order.OrderDate} | {order.TotalAmount:C} | {order.Customer?.Email} | {order.Status}");
        }
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static async Task DeleteOrderAsync()
    {
        using var db = new ShopContext();
        
        Console.WriteLine("Enter the OrderId of the product you want to delete:");
        if (!int.TryParse(Console.ReadLine(), out var orderId))
        {
            Console.WriteLine("Invalid OrderId!");
            return;
        }
        
        var order = await db.Categories.FindAsync(orderId);
        if (order == null)
        {
            Console.WriteLine("Order not found");
            return;
        }
        
        db.Categories.Remove(order);
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Order Deleted successfully!");
        }
        catch (DbUpdateException exception)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine("Something went wrong... :(");
        }
    }
    
}