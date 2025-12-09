namespace Databasprojekt.Helpers;
using Databasprojekt.Models;
using Microsoft.EntityFrameworkCore;
public class ProductHelper
{
    public static async Task ProductMenuAsync()
    {
        using var db = new ShopContext();
        
        Console.WriteLine("What would you like to do?");
        Console.WriteLine("1. Add a new product");
        Console.WriteLine("2. List all products");
        Console.WriteLine("3. Delete a product");
        Console.WriteLine("");
        
        var choice = Console.ReadKey().KeyChar;

        if (choice == '1')
        {
            await CreateProductAsync();
        }
        else if (choice == '2')
        {
            await ListProductsAsync();
        }
        else if (choice == '3')
        {
            await DeleteProductAsync();
        }
        else
        {
            Console.WriteLine("Invalid choice.");
        }
        
    }
    
    public static async Task CreateProductAsync()
    {
        using var db = new ShopContext(); 
        
        Console.WriteLine("Enter Product Name:");
        var productName = Console.ReadLine();

        if (string.IsNullOrEmpty(productName) || productName.Length > 100)
        {
            Console.WriteLine("Name cannot be empty or more than 100 characters");
        }
        
        Console.WriteLine("Enter price:");
        if (!decimal.TryParse(Console.ReadLine(), out var price))
        {
            Console.WriteLine("Invalid price");
            return;
        }
        if (price <= 0)
        {
            Console.WriteLine("Price has to be greater than zero");
            return;
        }
        
        Console.WriteLine("Enter quantity:");
        if (!int.TryParse(Console.ReadLine(), out var quantity))
        {
            Console.WriteLine("Invalid quantity");
            return;
        }

        if (quantity < 0)
        {
            Console.WriteLine("Quantity cannot be negative");
            return;
        }
        
        Console.WriteLine("Enter Product Description (optional):");
        var productDescription = Console.ReadLine();

        if (productDescription?.Length > 250)
        {
            Console.WriteLine("Description cannot be more than 250 characters");
        }

        await CategoryHelper.ListCategoriesAsync();
        Console.WriteLine("Enter the CategoryId belonging to the product:");
        var input = Console.ReadLine()?.Trim();
        
        if (!int.TryParse(input, out var categoryId) || string.IsNullOrEmpty(input) || categoryId < 0)
        {
            Console.WriteLine("CategoryId cannot be empty, negative and has to be a number");
            return;
        }
        
        var category = await db.Categories.FindAsync(categoryId);
        if (category == null)
        {
            Console.WriteLine("Category not found");
            return;
        }
        
        
        await db.AddAsync(new Product 
            {
                Name = productName, 
                Price = price, 
                StockQuantity = quantity, 
                Description = productDescription, 
                CategoryId = categoryId
            });
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Product Created!");
        }
        catch (DbUpdateException exception)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine("Something went wrong... :(");    
        }
    }
    
    public static async Task ListProductsAsync()
    {
        using var db = new ShopContext();
        
        var products = await db.Products
            .OrderBy(c => c.ProductId)
            .AsNoTracking()
            .Include(c => c.Category)
            .ToListAsync();
        Console.WriteLine("Products:");
        Console.WriteLine("ProductName | ProductId | Price | ProductDescription | Category");
        foreach (var product in products)
        {
            Console.WriteLine($"{product.Name} | {product.ProductId} | {product.Price} | {product.Description} | {product.Category?.CategoryName}");
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
    public static async Task DeleteProductAsync()
    {
        using var db = new ShopContext();
        
        Console.WriteLine("Enter the ProductId of the product you want to delete:");
        if (!int.TryParse(Console.ReadLine(), out var productId))
        {
            Console.WriteLine("Invalid ProductId!");
            return;
        }
        
        var product = await db.Categories.FindAsync(productId);
        if (product == null)
        {
            Console.WriteLine("Product not found");
            return;
        }
        
        db.Categories.Remove(product);
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Product Deleted successfully!");
        }
        catch (DbUpdateException exception)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine("Something went wrong... :(");
        }
    }
}