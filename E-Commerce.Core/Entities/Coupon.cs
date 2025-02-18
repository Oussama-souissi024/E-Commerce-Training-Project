using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Entities
{
    // The Coupon class represents a discount that can be applied to orders
    // It manages both the local database coupon and its corresponding Stripe coupon
    public class Coupon
    {
        // Unique identifier for the coupon in our database
        [Key]
        public int CouponId { get; set; }

        // The code that users enter to apply the coupon
        // Example: "SUMMER2024" or "WELCOME10"
        [Required]
        [MaxLength(100)]
        public string CouponCode { get; set; }

        // The amount of discount this coupon provides
        // Stored with 2 decimal places for precise calculations
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal DiscountAmount { get; set; }

        // Minimum order amount required to use this coupon
        // Ensures coupons are used only on qualifying orders
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal MinAmount { get; set; }

        // The ID of the corresponding coupon in Stripe
        // Optional field that links our coupon to Stripe's payment system
        [MaxLength(100)]
        public string? StripeId { get; set; }  // Store the Stripe coupon ID

        // Navigation properties to track where this coupon is used
        // One coupon can be used in multiple cart headers and order headers
        public ICollection<CartHeader> CartHeaders { get; set; }
        public ICollection<OrderHeader> OrderHeaders { get; set; }
    }
}
