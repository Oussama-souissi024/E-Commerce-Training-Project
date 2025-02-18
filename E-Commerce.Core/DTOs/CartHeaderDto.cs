using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Core.DTOs
{
    // DTO representing the header/summary information of a shopping cart
    // Contains cart identification, user details, and pricing information
    public class CartHeaderDto
    {
        // Unique identifier for the cart header
        public int CartHeaderId { get; set; }

        // ID of the user who owns this cart
        public string? UserId { get; set; }

        // Coupon code applied to this cart for discounts
        public string? CouponCode { get; set; }

        // Amount discounted from the cart total
        public decimal Discount { get; set; }

        // Total amount of all items in the cart after discount
        public decimal CartTotal { get; set; }

        // Customer contact information
        // Required for order processing and communication
        [Required]
        public string? Name { get; set; }

        // Customer phone number for order updates
        [Required]
        public string? Phone { get; set; }

        // Customer email for order confirmation and updates
        [Required]
        public string? Email { get; set; }
    }
}
