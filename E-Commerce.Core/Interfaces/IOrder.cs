using E_Commerce.Core.DTOs;
using E_Commerce.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Interfaces
{
    // Interface defining operations for managing orders
    // Handles order creation, retrieval, and status updates
    public interface IOrder
    {
        // Creates a new order from a shopping cart
        // Converts cart items to order items and initializes order header
        // Returns the created order or null if creation fails
        Task<OrderHeaderDto?> CreateOrderAsync(CartDto cartDto);

        // Retrieves all orders for a user based on their claims
        // For admin users, returns all orders
        // For regular users, returns only their orders
        Task<IEnumerable<OrderHeaderDto?>> GetAllOrderAsync(ClaimsPrincipal claimUser);

        // Retrieves a specific order by its ID
        // Returns null if order is not found
        // Includes all order details and related information
        Task<OrderHeaderDto?> GetOrderAsync(int orderId);

        // Updates the status of an order (e.g., Pending to Approved)
        // Returns the new status if successful, null if failed
        Task<string?> UpdateOrderStatusAsync(int orderId, string newStatus);
    }
}
