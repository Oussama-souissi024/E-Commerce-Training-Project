using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Utility
{
    // The StaticDetails class contains constant values used throughout the application
    // It centralizes important string values to avoid typos and make maintenance easier
    public class StaticDetails
    {
        // Order status constants
        // These define all possible states of an order in the system
        public const string Status_Pending = "Pending";        // Initial state when order is created
        public const string Status_Approved = "Approved";      // Order is approved after payment
        public const string Status_ReadyForPickup = "ReadyForPickup";  // Order is ready for customer pickup
        public const string Status_Completed = "Completed";    // Order has been delivered/picked up
        public const string Status_Refunded = "Refunded";      // Payment has been refunded
        public const string Status_Cancelled = "Cancelled";    // Order has been cancelled

        // User role constants
        // Define the different types of users in the system
        public const string RoleAdmin = "Admin";      // Administrative users with full access
        public const string RoleCustomer = "Customer"; // Regular customers with limited access
    }
}
