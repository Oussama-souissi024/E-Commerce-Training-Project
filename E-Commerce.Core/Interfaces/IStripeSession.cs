using E_Commerce.Core.DTOs;


namespace E_Commerce.Core.Interfaces
{
    // Interface for managing Stripe payment integration
    // Handles payment sessions, coupons, and refunds
    public interface IStripeSession
    {
        // Creates a new Stripe checkout session
        // Initializes payment intent and returns session details
        // Returns null if session creation fails
        Task<StripeRequestDto?> CreateStripeSessionAsync(StripeRequestDto stripeRequestDto);

        // Validates a completed Stripe payment session
        // Updates order status based on payment result
        // Returns updated order header information
        Task<OrderHeaderDto> ValidateStripeSession(int orderHeaderId);

        // Creates a new coupon in Stripe's system
        // Syncs local coupon with Stripe's coupon
        // Returns success/error message
        Task<string> AddStripeCoupon(CouponDto couponDto);

        // Removes a coupon from Stripe's system
        // Should be called when deleting local coupons
        // Returns success/error message
        Task<string> DeleteStripeCoupon(CouponDto couponDto);

        // Processes a refund for a completed payment
        // Used for order cancellations or returns
        // Returns true if refund is successful
        Task<bool> StripeRefundOptions(string paymentIntentId);
    }
}
