using Databasprojekt;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Db " + Path.Combine(AppContext.BaseDirectory, "Databasprojekt.db"));

await DbSeeding.SeedDbAsync();

while (true)
{
    
}