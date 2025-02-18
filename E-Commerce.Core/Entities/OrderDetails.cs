using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Entities
{
    // The OrderDetails class represents individual items in an order
    // It stores the specific products, their quantities, and prices at the time of purchase
    public class OrderDetails
    {
        // Unique identifier for this order detail entry
        [Key]
        public int OrderDetailsId { get; set; }

        // Foreign key linking to the order header
        // Each order detail must belong to an order
        [Required]
        [ForeignKey("OrderHeader")]
        public int OrderHeaderId { get; set; }

        // Foreign key linking to the product
        // Identifies which product was ordered
        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        // Name of the product at the time of purchase
        // Stored separately in case product name changes later
        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; }

        // Price of the product at the time of purchase
        // Stored with 2 decimal places
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        // Quantity of the product ordered
        public int Count { get; set; }

        // Navigation properties for related entities
        // Enables easy access to order header and product information
        public OrderHeader OrderHeader { get; set; }
        public Product Product { get; set; }
    }
}
