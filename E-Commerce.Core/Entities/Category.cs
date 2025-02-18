using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Entities
{
    // The Category class represents a grouping of products
    // It helps organize products into logical groups like "Electronics", "Clothing", etc.
    public class Category
    {
        // Unique identifier for the category in the database
        [Key]
        public int CategoryID { get; set; }

        // The name of the category, limited to 100 characters
        // Required field - cannot be null or empty
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Navigation property to access all products in this category
        // One category can have multiple products (one-to-many relationship)
        public ICollection<Product> Products { get; set; }
    }
}
