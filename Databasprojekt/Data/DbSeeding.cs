namespace Databasprojekt;
using Databasprojekt.Models;
using Microsoft.EntityFrameworkCore;

public class DbSeeding
{
    public static async Task SeedDbAsync()
    {
        using (var db = new ShopContext())
        {
            await db.Database.MigrateAsync();

            if (!await db.Customers.AnyAsync())
            {
                db.Customers.AddRange(
                    new Customer {Name = "John Doe", Email = "john.doe@gmail.com", City = "Stockholm"},
                    new Customer {Name = "Elias Larsson", Email = "elias.larsson@gmail.com", City = "Åkersberga"}
                    );
                await db.SaveChangesAsync();
                Console.WriteLine("Customer(s) seeded in db!");
            }

            if (!await db.Categories.AnyAsync())
            {
                db.Categories.AddRange(
                    new Category {CategoryName = "Dairy", Description =  "Local dairy products"},
                    new Category {CategoryName = "Sweets & Candy", Description =  "Sortiment of different sweets & candy"}
                    );
                await db.SaveChangesAsync();
                Console.WriteLine("Category(s) seeded in db!");
            }
            
            var dairy = await db.Categories.FirstAsync(c => c.CategoryName == "Dairy");
            var sweets = await db.Categories.FirstAsync(c => c.CategoryName == "Sweets & Candy");
            
            if (!await db.Products.AnyAsync())
            {
                db.Products.AddRange(
                    new Product {Name = "Milk", Price = 13, Description = "Milk from ARLA", CategoryId = dairy.CategoryId},
                    new Product {Name = "Chips", Price = 25, Description = "Sourcream & Onion chips from estrella", CategoryId = sweets.CategoryId},
                    new Product {Name = "Cheese", Price = 100, Description = "Six months aged cheese", CategoryId = dairy.CategoryId}
                    );
                await db.SaveChangesAsync();
                Console.WriteLine("Product(s) seeded in db!");
            }
        }
    }
}