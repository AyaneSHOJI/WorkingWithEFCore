using Packt.Shared;
using Microsoft.EntityFrameworkCore; // Include extension method
using static System.Console;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.ChangeTracking; // CollectionEntry

//WriteLine($"Using {ProjectConstants.DatabaseProvider} database provider");
//QueryingCategories();
//FilteredInclude();
//QueryingProducts();
//QueryWithLike();

//if(AddProduct(categoryId: 6, productName: "Bob's Burgers", price: 500M))
//{
//    WriteLine("Add product successful.");
//}

if (IncreaseProductPrice(productNameStartWith: "Bob", amount: 20M))
{
    WriteLine("Update product price successful.");
}
    
ListProducts();

static void QueryingCategories()
{
    using (Northwind db = new())
    { 
        ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
        loggerFactory.AddProvider(new ConsoleLoggerProvider());

        WriteLine("Categories and how many products they have:");

        // a query ti get all categories and their related products
        IQueryable<Category>? categories; //= db.Categories; //.Include(c => c.Products);   

        db.ChangeTracker.LazyLoadingEnabled = false;

        Write("Enable eager loading ? (Y/N): ");
        bool eagerloading = (ReadKey().Key == ConsoleKey.Y);
        bool explicitloading = false;
        WriteLine();

        if (eagerloading)
        {
            categories = db.Categories?.Include(c => c.Products);
        }
        else
        {
            categories = db.Categories;

            Write("Enable explicit loading? (Y/N): ");
            explicitloading = (ReadKey().Key == ConsoleKey.Y);
            WriteLine();
        }

        if(categories is null)
        {
            WriteLine("No categories found");
        }

        // execute query and enumerate results
        foreach(Category c in categories)
        {
            Write($"Explicitly load products for {c.CategoryName}? (Y/N): ");
            ConsoleKeyInfo key = ReadKey();
            WriteLine();
            if(key.Key == ConsoleKey.Y)
            {
                CollectionEntry<Category, Product> products =
                    db.Entry(c).Collection(c2 => c2.Products);
                if(!products.IsLoaded)products.Load();
            }
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
            // to add query tag in log message
            .TagWith("Products filtered by price and sorted.")
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

static void QueryWithLike()
{
    using(Northwind db = new())
    {
        ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
        loggerFactory.AddProvider(new ConsoleLoggerProvider());

        Write("Enter part of a product name: ");
        string? input = ReadLine();

        IQueryable<Product>? products = db.Products?
            // Like Summary:
            //     An implementation of the SQL LIKE operation. On relational databases this is
            //     usually directly translated to SQL.
            //     Note that the semantics of the comparison will depend on the database configuration.
            //     In particular, it may be either case-sensitive or case-insensitive.
            //%	= Any string of zero or more characters.	title like '%computer%' finds all titles with the word "computer" anywhere in the title.
            .Where(p => EF.Functions.Like(p.ProductName, $"%{input}%"));

        if(products is null)
        {
            WriteLine("No products found.");
            return;
        }

        foreach(Product p in products)
        {
            WriteLine("{0} has {1} units in stock. Discontinued? {2}",
                p.ProductName, p.Stock, p.Discontinued);
        }
    }
}

static bool AddProduct(int categoryId, string productName, decimal? price)
{
    using(Northwind db = new())
    {
        Product p = new()
        {
            CategoryId = categoryId,
            ProductName = productName,
            Cost = price
        };

        // mark product as added in change tracking
        db.Products.Add(p);

        // save tracked chang to database
        int affected = db.SaveChanges();
        return (affected == 1);
    }
}

static void ListProducts()
{
    using(Northwind db = new())
    {
        WriteLine("{0, -3} {1, -35} {2,8} {3,5} {4}",
            "Id", "Product Name", "Cost", "Stock", "Disc.");

        foreach(Product p in db.Products
            .OrderByDescending(product => product.Cost))
        {
            WriteLine("{0:000} {1, -35} {2,8:$#,##0.00} {3,5} {4}",
                p.ProductId, p.ProductName, p.Cost, p.Stock, p.Discontinued);
        }
    }
}

static bool IncreaseProductPrice(string productNameStartWith, decimal amount)
{
    using(Northwind db = new())
    {
        // get first product whose name starts with name
        Product updateProduct = db.Products.First(
            p => p.ProductName.StartsWith(productNameStartWith));

        updateProduct.Cost += amount;

        int affected = db.SaveChanges();
        return(affected == 1);
    }
}