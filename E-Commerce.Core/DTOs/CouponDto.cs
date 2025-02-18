using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.DTOs
{
    // DTO for handling coupon operations
    // Used for creating, applying, and managing discount coupons
    public class CouponDto
    {
        // The code that users enter to apply the coupon
        // Example: "SUMMER2024" or "WELCOME10"
        public string CouponCode { get; set; }

        // Amount of discount this coupon provides
        // Can be a fixed amount or percentage off
        public double DiscountAmount { get; set; }

        // Minimum order amount required to use this coupon
        // Ensures coupons are used only on qualifying orders
        public int MinAmount { get; set; }

        // Optional ID field, used when updating existing coupons
        // Null when creating a new coupon
        public int? CouponId { get; set; }
    }
}
