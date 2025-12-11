namespace Databasprojekt.Helpers;
using Microsoft.EntityFrameworkCore;
using Databasprojekt.Models;

public class CategoryHelper
{
    public static async Task CategoryMenuAsync()
    {
        using var db = new ShopContext();
        
        Console.WriteLine("What would you like to do?");
        Console.WriteLine("1. Add a new category");
        Console.WriteLine("2. List all categories");
        Console.WriteLine("3. Edit category");
        Console.WriteLine("4. Delete category");
        Console.WriteLine("");
        
        var choice = Console.ReadKey().KeyChar;

        if (choice == '1')
        {
            await CreateCategoryAsync();
        }
        else if (choice == '2')
        {
            await ListCategoriesAsync();
        }
        else if (choice == '3')
        {
            await EditCategoryAsync();
        }
        else if (choice == '4')
        {
            await DeleteCategoryAsync();
        }
        else
        {
            Console.WriteLine("Invalid choice.");
        }
        
    }
    public static async Task CreateCategoryAsync()
    {
        using var db = new ShopContext();

        await AdminHelper.AdminCheckAsync();
        
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
            else 
            {
                var selectedCategory = await db.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.CategoryId == int.Parse(categoryId));

                if (selectedCategory == null)
                {
                    Console.WriteLine("Category not found");
                    return;
                }
                
                Console.WriteLine($"Here are the products under the Category: {selectedCategory.CategoryName}");
                foreach (var product in selectedCategory.Products)
                {
                    Console.WriteLine($"- {product.ProductId} | {product.Name}");
                }
            }
        }
    }

    public static async Task EditCategoryAsync()
    {
        using var db = new ShopContext();
        
        await AdminHelper.AdminCheckAsync();
        
        Console.WriteLine("Enter CategoryId of the category you want to edit:");
        if (!int.TryParse(Console.ReadLine(), out var categoryId))
        {
            Console.WriteLine("Invalid CategoryId!");
        }
        
        var category = await db.Categories.FindAsync(categoryId);
        if (category == null)
        {
            Console.WriteLine("Category not found");
        }
        Console.WriteLine("Category Name | CategoryId | CategoryDescription");
        Console.WriteLine($"{category?.CategoryName} | {category?.CategoryId} | {category?.Description}");
        Console.WriteLine($"Products under  the Category: {category?.CategoryName}");
        Console.WriteLine("ProductId | Name | Price | Description");
        foreach (var product in category?.Products)
        {
            Console.WriteLine($"- {product.ProductId} | {product.Name} | {product.Price:C} | {product.Description}");
        }

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("1. Change Category Name");
            Console.WriteLine("2. Change Category Description");
            Console.WriteLine("3. Exit to Menu");
            Console.WriteLine("");
            var choice = Console.ReadKey().KeyChar;

            switch (choice)
            {
                case '1':
                    Console.WriteLine("Enter new name:");
                    var newName = Console.ReadLine();
                    if (string.IsNullOrEmpty(newName))
                    {
                        Console.WriteLine("Name can not be empty");
                    }
                    else
                    {
                        Console.WriteLine($"Category Name changed from {category.CategoryName} to {newName}");
                        category.CategoryName = newName;
                    }
                    break;
                case '2':
                    Console.WriteLine("Enter new description:");
                    var newDescription = Console.ReadLine();
                    if (string.IsNullOrEmpty(newDescription))
                    {
                        Console.WriteLine("Description can not be empty");
                    }
                    else
                    {
                        Console.WriteLine($"Description changed from {category.Description} to {newDescription}");
                        category.Description = newDescription;
                    }
                    break;
                case '3':
                    Console.WriteLine("Returning to Menu...");
                    return;
            }
        }
    }
    
    public static async Task DeleteCategoryAsync()
    {
        using var db = new ShopContext();
        
        await AdminHelper.AdminCheckAsync();
        
        Console.WriteLine("Enter the CategoryId of the category you want to delete:");
        if (!int.TryParse(Console.ReadLine(), out var categoryId))
        {
            Console.WriteLine("Invalid CategoryId!");
            return;
        }
        
        var category = await db.Categories.FindAsync(categoryId);
        if (category == null)
        {
            Console.WriteLine("Category not found");
            return;
        }
        
        db.Categories.Remove(category);
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Category Deleted successfully!");
        }
        catch (DbUpdateException exception)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine("Something went wrong... :(");
        }
    }
}