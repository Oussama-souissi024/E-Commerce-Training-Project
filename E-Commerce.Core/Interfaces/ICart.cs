using E_Commerce.Core.DTOs;
using E_Commerce.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Interfaces
{
    // Interface defining operations for managing shopping carts
    // Handles cart creation, updates, and coupon applications
    public interface ICart
    {
        // Retrieves a user's shopping cart by their ID
        // Returns null if no cart exists for the user
        // Includes all cart items and applied discounts
        Task<CartDto?> GetCartByIdsync(string UserId);

        // Creates or updates a shopping cart
        // Handles both adding new items and updating quantities
        // Returns success/error message
        Task<string?> UpsertCartAsync(CartDto cartDto);

        // Applies a coupon code to the cart
        // Updates cart total with the discount
        // Returns success/error message
        Task<string?> ApplyCouponAsync(CartDto cartDto);

        // Removes a specific item from the cart
        // cartDetailsId identifies the specific cart item
        // Returns success/error message
        Task<string?> RemoveCart(int cartDetailsId);

        // Removes all items from a user's cart
        // Used after order completion or on user request
        // Returns success/error message
        Task<string?> ClearCart(string userId);
    }
}
