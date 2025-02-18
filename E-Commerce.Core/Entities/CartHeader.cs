using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Entities
{
    // The CartHeader class represents the main shopping cart entity for a user
    // It contains the cart's general information and links to its details
    public class CartHeader
    {
        // Unique identifier for the shopping cart header
        [Key]
        public int CartHeaderId { get; set; }

        // ID of the user who owns this shopping cart
        // Required field linking to ApplicationUser
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        // Optional coupon code applied to this cart
        // Can be null if no coupon is applied
        public string? CouponCode { get; set; }

        // Navigation properties for related entities
        // Links to the user who owns the cart
        public ApplicationUser User { get; set; }

        // Collection of items in the cart
        // One cart header can have multiple cart details (products)
        public ICollection<CartDetails> CartDetails { get; set; }
    }
}
