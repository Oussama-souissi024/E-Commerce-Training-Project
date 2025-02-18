using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace E_Commerce.Core.DTOs
{
    // DTO for transferring product information between layers
    // Used for both creating new products and displaying existing ones
    public class ProductDto
    {
        // Unique identifier for the product
        public int ProductID { get; set; }

        // Name of the product, limited to 200 characters
        [MaxLength(200)]
        public string Name { get; set; }

        // Price of the product with 2 decimal places
        // Uses decimal for precise monetary calculations
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        // Detailed description of the product
        // Optional field that can be null
        public string? Description { get; set; }

        // ID of the category this product belongs to
        // Optional to allow product creation before category assignment
        public int? CategoryID { get; set; }

        // URL where the product's image is stored
        // Optional field for products without images
        public string? ImageUrl { get; set; }

        // Quantity of the product
        // Defaults to 1 for new products
        public int Count { get; set; } = 1;

        // Uploaded image file for the product
        // Used when creating or updating product images
        public IFormFile? ImageFile { get; set; }
    }
}
