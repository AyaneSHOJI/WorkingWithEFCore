using Packt.Shared;
using Microsoft.EntityFrameworkCore; // Include extension method
using static System.Console;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

//WriteLine($"Using {ProjectConstants.DatabaseProvider} database provider");
//QueryingCategories();
//FilteredInclude();
QueryingProducts();


static void QueryingCategories()
{
    using (Northwind db = new())
    {
        ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
        loggerFactory.AddProvider(new ConsoleLoggerProvider());

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

static void FilteredInclude()
{
    using (Northwind db = new())
    {
        ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
        loggerFactory.AddProvider(new ConsoleLoggerProvider());

        Write("Enter a minimum for units in stock: ");
        string unitsInStock = ReadLine() ?? "10";
        int stock = int.Parse(unitsInStock);

        IQueryable<Category>? categories = db.Categories?.Include(c => c.Products.Where(p => p.Stock >= stock));

        if(categories is null)
        {
            WriteLine("No categories found.");
            return;
        }

        // get the generated SQL statement with ToQueryString
        WriteLine($"ToQueryString : {categories.ToQueryString()}");

        foreach(Category c in categories)
        {
            WriteLine($"{c.CategoryName} has {c.Products.Count} products with a minimum of {stock} units in stock");
            
            foreach(Product p in c.Products)
            {
                WriteLine($"   {p.ProductName} has {p.Stock} units in stock.");
            }
        }
    }
}

static void QueryingProducts()
{
    using(Northwind db = new())
    {
        ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
        loggerFactory.AddProvider(new ConsoleLoggerProvider());

        WriteLine("Products that cost more than a price, highest at top.");
        string? input;
        decimal price;

        // input price must be a valid price
        do
        {
            Write("Enter a product price: ");
            input = ReadLine();
        } while(!decimal.TryParse(input, out price));

        IQueryable<Product>? products = db.Products?
            .Where(p => p.Cost > price)
            .OrderByDescending(p => p.Cost);

        if(products is null)
        {
            WriteLine("No products found");
            return;
        }

        foreach(Product p in products)
        {
            // numeric format with $, 3 digits with separated by point
            WriteLine("{0}: {1} costs {2:$#,##0.00} and has {3} in stock",
                p.ProductId, p.ProductName, p.Cost, p.Stock);
        }
     
    }
}

