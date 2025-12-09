namespace Databasprojekt.Helpers;
using Databasprojekt.Models;
using Microsoft.EntityFrameworkCore;

public class CustomerHelper
{
    public static async Task CustomerMenuAsync()
    {
        using var db = new ShopContext();
        
        Console.WriteLine("What would you like to do?");
        Console.WriteLine("1. Add a new customer");
        Console.WriteLine("2. List all customers");
        Console.WriteLine("3. Delete a Customer");
        Console.WriteLine("");
        
        var choice = Console.ReadKey().KeyChar;

        if (choice == '1')
        {
            await CreateCustomerAsync();
        }
        else if (choice == '2')
        {
            await ListCustomersAsync();
        }
        else if (choice == '3')
        {
            await DeleteCustomerAsync();
        }
        else
        {
            Console.WriteLine("Invalid choice.");
        }
        
    }
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

    public static async Task DeleteCustomerAsync()
    {
        using var db = new ShopContext();
        
        Console.WriteLine("Enter customer id of the customer that you want to erase:");
        if (!int.TryParse(Console.ReadLine(), out var customerId))
        {
            Console.WriteLine("Invalid CustomerId");
            return;
        }
        
        var customer = await db.Customers.FindAsync(customerId);
        if (customer == null)
        {
            Console.WriteLine("Customer not found");
            return;
        }
        
        db.Customers.Remove(customer);
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Customer deleted!");
        }
        catch (DbUpdateException exception)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine("Something went wrong... :(");
        }
    }
}