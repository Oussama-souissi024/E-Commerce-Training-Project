using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Entities
{
    // The CartDetails class represents individual items in a shopping cart
    // It links products to cart headers and tracks quantity of each product
    public class CartDetails
    {
        // Unique identifier for this cart detail entry
        [Key]
        public int CartDetailsId { get; set; }

        // Foreign key linking to the cart header
        // Each cart detail must belong to a cart header
        [Required]
        [ForeignKey("CartHeader")]
        public int CartHeaderId { get; set; }

        // Foreign key linking to the product
        // Identifies which product is in the cart
        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        // Quantity of the product in the cart
        // Must be between 1 and 100 items
        [Required]
        [Range(1, 100, ErrorMessage = "The value must be between 1 and 100.")]
        public int Count { get; set; }

        // Navigation properties for related entities
        // Enables easy access to the cart header and product information
        public CartHeader CartHeader { get; set; }
        public Product Product { get; set; }
    }
}
