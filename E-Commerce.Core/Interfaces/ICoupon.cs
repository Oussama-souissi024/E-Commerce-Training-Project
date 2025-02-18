using E_Commerce.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Interfaces
{
    // Interface defining operations for managing discount coupons
    // Handles creation, validation, and tracking of promotional codes
    public interface ICoupon
    {
        // Adds a new coupon to the system
        // Returns the created coupon with its generated ID
        Task<Coupon> AddAsync(Coupon coupon);

        // Retrieves a specific coupon by its ID
        // Returns null if coupon is not found
        Task<Coupon> ReadByIdAsync(int id);

        // Retrieves a coupon by its code (e.g., "SUMMER2024")
        // Used when validating coupon codes during checkout
        // Returns null if coupon code doesn't exist
        Task<Coupon> ReadByCouponCodeAsync(string couponCode);

        // Retrieves all available coupons
        // Used for coupon management in admin panel
        Task<IEnumerable<Coupon>> ReadAllAsync();

        // Updates an existing coupon's information
        // Used to modify discount amount, validity, etc.
        void Update(Coupon coupon);

        // Removes a coupon from the system
        // Should handle cases where coupon is in use
        Task DeleteAsync(int id);
    }
}
