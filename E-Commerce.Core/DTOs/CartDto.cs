namespace E_Commerce.Core.DTOs
{
    // DTO representing a complete shopping cart
    // Combines cart header information with its detailed items
    public class CartDto
    {
        // General information about the cart (user, coupon, etc.)
        // Contains the cart's metadata and summary information
        public CartHeaderDto CartHeader { get; set; }

        // Collection of items in the cart
        // Each item contains product details and quantity
        // Nullable because a cart might be empty
        public IEnumerable<CartDetailsDto>? CartDetails { get; set; }
    }
}
