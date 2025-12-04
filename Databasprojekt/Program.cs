

using Databasprojekt;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Db " + Path.Combine(AppContext.BaseDirectory, "Databasprojekt.db"));

using (var db = new ShopContext())
{
    await db.Database.MigrateAsync();
    
    
}