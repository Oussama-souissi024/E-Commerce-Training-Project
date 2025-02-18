using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Core.Entities
{
    // The ApplicationUser class extends IdentityUser to add custom user properties
    // IdentityUser already includes properties like Email, UserName, PasswordHash, etc.
    public class ApplicationUser : IdentityUser
    {
        // Display name of the user
        // Required field with maximum length of 100 characters
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // User's postal code for shipping and billing
        public int ZipCode { get; set; }

        // Navigation properties for related entities
        // Links to user's shopping carts
        // One user can have multiple shopping carts over time
        public ICollection<CartHeader> CartHeaders { get; set; }

        // Links to user's orders
        // One user can have multiple orders
        public ICollection<OrderHeader> OrderHeaders { get; set; }
    }
}
