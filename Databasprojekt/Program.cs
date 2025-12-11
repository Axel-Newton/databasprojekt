using Databasprojekt.Data;
using Databasprojekt.Helpers;

Console.WriteLine("Db " + Path.Combine(AppContext.BaseDirectory, "databasprojekt.db"));

await DbSeeding.SeedDbAsync();

while (true)
{
    Console.WriteLine("Welcome to Axel's Shop!");
    Console.WriteLine("1. Categories");
    Console.WriteLine("2. Products");
    Console.WriteLine("3. Customers");
    Console.WriteLine("4. Orders");
    Console.WriteLine("5. Exit");
    Console.WriteLine("");
    
    var choice = Console.ReadKey().KeyChar;

    switch (choice)
    {
        case '1':
            await CategoryHelper.CategoryMenuAsync();
            break;
        case '2':
            await ProductHelper.ProductMenuAsync();
            break;
        case '3':
            await CustomerHelper.CustomerMenuAsync();
            break;
        case '4':
            await OrderHelper.OrderMenuAsync();
            break;
        case '5':
            Console.WriteLine("Exiting program...");
            return;
        default:
            Console.WriteLine("Invalid choice");
            break;
    }
}