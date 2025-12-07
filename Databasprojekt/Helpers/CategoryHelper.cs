namespace Databasprojekt.Helpers;
using Microsoft.EntityFrameworkCore;
using Databasprojekt.Models;

public class CategoryHelper
{
    public static async Task CreateCategoryAsync()
    {
        using var db = new ShopContext(); 
        
        Console.WriteLine("Enter Category Name:");
        var categoryName = Console.ReadLine();

        if (string.IsNullOrEmpty(categoryName) || categoryName.Length > 100)
        {
            Console.WriteLine("Name cannot be empty or more than 100 characters");
        }
        
        Console.WriteLine("Enter Category Description (optional):");
        var categoryDescription = Console.ReadLine();

        if (categoryDescription?.Length > 250)
        {
            Console.WriteLine("Description cannot be more than 250 characters");
        }
        
        await db.AddAsync(new Category {CategoryName = categoryName,  Description = categoryDescription});
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Category Created!");
        }
        catch (DbUpdateException exception)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine("Something went wrong... :(");    
        }
    }

    public static async Task ListCategoriesAsync()
    {
        using var db = new ShopContext();
        
        var categories = await db.Categories
            .OrderBy(c => c.CategoryName)
            .AsNoTracking()
            .Include(c => c.Products)
            .ToListAsync();
        Console.WriteLine("Categories:");
        Console.WriteLine("Category Name | CategoryId | CategoryDescription | Products");
        foreach (var category in categories)
        {
            Console.WriteLine($"{category.CategoryName} | {category.CategoryId} | {category.Description}");
        }
        
        Console.WriteLine("View category products? (y/n)");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            Console.WriteLine("Enter CategoryId:");
            var categoryId = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(categoryId))
            {
                Console.WriteLine("CategoryId can not be empty");
            }
            
        }
    }
}