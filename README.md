# Databasprojekt
An E-commerse console application and admin demo built with C# and Entity Framework Core (SQLite) with simple management for Categories, Products, Customer and Orders.
## Features
* CRUD for Categories, Products and Customers
* Create and list Orders with automatic OrderRows and stock updates
* XOR-based EncryptionHelper demo
* Database seeding
## Prerequisites
* .NET 8 SDK
* JetBrains Rider or any IDE / terminal on Windows
* dotnet-ef for applying migrations from CLI
## Quick start (IDE)
1. Open the solution in an IDE
2. Build the project
3. Run the project (DB file created automatically on startup)
4. Use the console menu to manage entities
## Quick start (CLI)
From the prject root:
````
dotnet build
dotnet run --project Databasprojekt
````
If you use EF migrations from CLI: 
````
dotnet tool install --global dotnet-ef # if not installed
dotnet ef database update --project Databasprojekt
dotnet run --project Databasprojekt
````
## Database
* SQLite database file named databasprojekt.db is created in the app base directory (printed at startup)
* Migrations exists in Databasprojekt/Migrations
* Seed logic exists in Databasprojekt/Data/DbSeeding.cs

To clean databse during development, delete the DB file or call db.Database.EnsureDeleted() before seeding.