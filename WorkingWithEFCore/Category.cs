using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema; // [Column]

namespace Packt.Shared;

public class Category
{
    // these properties map to colums in the database
    public int CategoryID { get; set; }
    public string? CategoryName { get; set; }
    
    [Column(TypeName = "ntext")]
    public string? Description { get; set; }

    // define a navifation property for related rows
    public virtual ICollection<Product> Products { get; set; }

    public Category()
    {
        // to enable developers to add products to a Category we must intialize the navigation property to an empty collection
        // HashSet<T> Summary:
        //     Initializes a new instance of the System.Collections.Generic.HashSet`1 class
        //     that is empty and uses the default equality comparer for the set type.
        Products = new HashSet<Product>();
    }
}
