using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.DTOs
{
    // DTO for transferring category information
    // Used for creating, updating, and displaying product categories
    public class CategoryDto
    {
        // Unique identifier for the category
        // Maps to the CategoryID in the database
        public int CategoryID { get; set; }

        // Name of the category (e.g., "Electronics", "Books", etc.)
        // Used to group and organize products
        public string Name { get; set; }
    }
}
