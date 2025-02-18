using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Entities
{
    // The Product class represents an item that can be sold in our e-commerce system
    // It contains all the essential information about a product including its price, description, and category
    public class Product
    {
        // Unique identifier for the product in the database
        [Key]
        public int ProductID { get; set; }

        // The name of the product, limited to 200 characters
        // Required field - cannot be null or empty
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        // The price of the product with 2 decimal places
        // Uses decimal for precise monetary calculations
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        // Detailed description of the product
        // Optional field - can be null
        public string Description { get; set; }

        // Foreign key linking to the Category table
        // Each product must belong to a category
        [Required]
        [ForeignKey("Category")]
        public int CategoryID { get; set; }

        // URL to the product's image
        // Optional field - can be null
        public string ImageUrl { get; set; }

        // Navigation property to access the associated Category
        // Enables easy access to category information through Entity Framework
        public Category Category { get; set; }

        // Navigation properties for related cart and order details
        // One product can be in multiple shopping carts and orders
        public ICollection<CartDetails> CartDetails { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
