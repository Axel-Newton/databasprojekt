namespace Databasprojekt.Helpers;
using Databasprojekt.Models;
using Microsoft.EntityFrameworkCore;

public class CustomerHelper
{
    public static async Task CreateCustomerAsync()
    {
        using var db = new ShopContext();
        
        Console.WriteLine("Enter customer name:");
        var name = Console.ReadLine();

        if (string.IsNullOrEmpty(name) || name.Length > 100) 
        {
            Console.WriteLine("Customer name cannot be empty or more than 100 characters");
        }
        
        Console.WriteLine("Enter customer email:");
        var email = Console.ReadLine();

        if (string.IsNullOrEmpty(email) || email.Length > 100)
        {
            Console.WriteLine("Customer email cannot be empty or more than 100 characters");
        }
        
        Console.WriteLine("Enter customer city:");
        var city = Console.ReadLine();

        if (city?.Length > 100)
        {
            Console.WriteLine("Customer city cannot be more than 100 characters");
        }
        
        await db.Customers.AddAsync(new Customer{Name = name, Email = email, City = city});
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Customer created!");
        }
        catch (DbUpdateException exception)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine("Something went wrong... :(");
        }
    }

    public static async Task ListCustomersAsync()
    {
        using var db = new ShopContext();
        var customers = await db.Customers
            .OrderBy(o => o.CustomerId)
            .AsNoTracking()
            .Include(customer => customer.Orders)
            .ToListAsync();
        Console.WriteLine($"Customers count: {customers.Count}");
        Console.WriteLine("List of Customers:");
        Console.WriteLine("CustomerId | Name | Email | City | OrderCount");
        if (!customers.Any())
        {
            Console.WriteLine("No customers found!");
        }
        foreach (var customer in customers)
        {
            Console.WriteLine($"{customer.CustomerId} | {customer.Name}  | {customer.Email} | {customer.City} | {customer.Orders.Count}");
        }
    }
    
}