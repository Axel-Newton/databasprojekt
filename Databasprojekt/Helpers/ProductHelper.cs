using System.Diagnostics;
using Databasprojekt.Models;
using Microsoft.EntityFrameworkCore;

namespace Databasprojekt.Helpers;

public class ProductHelper
{
    public static async Task ProductMenuAsync()
    {
        using var db = new ShopContext();
        
        Console.WriteLine("What would you like to do?");
        Console.WriteLine("1. Add a new product");
        Console.WriteLine("2. List all products");
        Console.WriteLine("3. Edit product");
        Console.WriteLine("4. Delete a product");
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
            await EditProductAsync();
        }
        else if (choice == '4')
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
        
        await AdminHelper.AdminCheckAsync();
        
        Console.WriteLine("Enter Product Name:");
        var productName = Console.ReadLine();

        if (string.IsNullOrEmpty(productName) || productName.Length > 100)
        {
            Console.WriteLine("Name cannot be empty or more than 100 characters");
            return;
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
        
        Console.WriteLine("Enter product quantity:");
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
            return;
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
        
        var sq = Stopwatch.StartNew();
        
        var products = await db.Products
            .OrderBy(c => c.ProductId)
            .AsNoTracking()
            .Include(c => c.Category)
            .ToListAsync();
        Console.WriteLine("Products:");
        Console.WriteLine("ProductName | ProductId | Price | ProductDescription | StockQuantity | Category");
        foreach (var product in products)
        {
            Console.WriteLine($"{product.Name} | {product.ProductId} | {product.Price} | {product.Description} | {product.StockQuantity} |{product.Category?.CategoryName}");
        }
        sq.Stop();
        Console.WriteLine($"Total Time: {sq.ElapsedMilliseconds} ms");
        
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
    
    public static async Task EditProductAsync()
    {
        using var db = new ShopContext();
        
        await AdminHelper.AdminCheckAsync();
        
        Console.WriteLine("Enter ProductId of the product you want to edit:");
        if (!int.TryParse(Console.ReadLine(), out var productId))
        {
            Console.WriteLine("Invalid ProductId!");
        }
        
        var chosenProduct = await db.Products.FindAsync(productId);
        if (chosenProduct == null)
        {
            Console.WriteLine("Product not found");
        }
        Console.WriteLine("ProductId | Name | Price | Description");
        Console.WriteLine($"- {chosenProduct?.ProductId} | {chosenProduct?.Name} | {chosenProduct?.Price:C} | {chosenProduct?.Description}");
        

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("1. Change product Name");
            Console.WriteLine("2. Change product Price");
            Console.WriteLine("3. Change product StockQuantity");
            Console.WriteLine("4. Change product Description");
            Console.WriteLine("5. Return to Menu");
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
                        Console.WriteLine($"Product Name changed from {chosenProduct?.Name} to {newName}");
                        chosenProduct.Name = newName;
                    }
                    break;
                case '2':
                    Console.WriteLine("Enter new price:");
                    var newPrice = decimal.Parse(Console.ReadLine());
                    if (newPrice < 0)
                    {
                        Console.WriteLine("Price can not be less than zero");
                    }
                    else
                    {
                        Console.WriteLine($"Product Price changed from {chosenProduct?.Price:C} to {newPrice:C}");
                        chosenProduct.Price = newPrice;
                    }
                    break;
                case '3':
                    Console.WriteLine("Enter new stockquantity:");
                    var newStockQuantity = int.Parse(Console.ReadLine());
                    if (newStockQuantity < 0)
                    {
                        Console.WriteLine("StockQuantity can not be less than zero");
                    }
                    else
                    {
                        Console.WriteLine($"StockQuantity changed from {chosenProduct?.StockQuantity} to {newStockQuantity}");
                        chosenProduct.StockQuantity = newStockQuantity;
                    }
                    break;
                case '4':
                    Console.WriteLine("Enter new description:");
                    var newDescription = Console.ReadLine();
                    if (string.IsNullOrEmpty(newDescription))
                    {
                        Console.WriteLine("Description can not be empty");
                    }
                    else
                    {
                        Console.WriteLine($"Description changed from {chosenProduct?.Description} to {newDescription}");
                        chosenProduct.Description = newDescription;
                    }
                    break;
                case '5':
                    Console.WriteLine("Returning to Menu...");
                    return;
            }
            
        }
    }
    
    public static async Task DeleteProductAsync()
    {
        using var db = new ShopContext();
        
        await AdminHelper.AdminCheckAsync();
        
        Console.WriteLine("Enter the ProductId of the product you want to delete:");
        if (!int.TryParse(Console.ReadLine(), out var productId))
        {
            Console.WriteLine("Invalid ProductId!");
            return;
        }
        
        var product = await db.Products.FindAsync(productId);
        if (product == null)
        {
            Console.WriteLine("Product not found");
            return;
        }
        
        db.Products.Remove(product);
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