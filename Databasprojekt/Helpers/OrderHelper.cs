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
            await ListOrderSummaryAsync();
        }
        else if (choice == '4')
        {
            //await DeleteOrderAsync();
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
            if (quantity == null)
            {
                Console.WriteLine("Invalid quantity!, can not be empty");
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