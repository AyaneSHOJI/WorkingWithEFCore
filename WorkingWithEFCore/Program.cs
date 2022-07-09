using Packt.Shared;
using Microsoft.EntityFrameworkCore; // Include extension method
using static System.Console;

Console.WriteLine($"Using {ProjectConstants.DatabaseProvider} database provider");
queryingCategories();

static void queryingCategories()
{
    using (Northwind db = new())
    {
        WriteLine("Categories and how many products they have:");

        // a query ti get all categories and their related products
        IQueryable<Category>? categories = db.Categories?.Include(c => c.Products);   

        if(categories is null)
        {
            WriteLine("No categories found");
        }

        // execute query and enumerate results
        foreach(Category c in categories)
        {
            WriteLine($"{c.CategoryName} has {c.Products.Count} products");
        }

    }
}

