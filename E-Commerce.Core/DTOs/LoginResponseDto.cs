using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.DTOs
{
    // DTO for sending user information after successful login
    // Contains essential user details needed by the client application
    public class LoginResponseDto
    {
        // Unique identifier of the authenticated user
        // This is typically the user's ID from the database
        public string ID { get; set; }

        // User's email address
        // Used for communication and identification
        public string Email { get; set; }

        // User's display name in the application
        // Used for personalization throughout the UI
        public string Name { get; set; }

        // User's role in the system (e.g., "Admin" or "Customer")
        // Used for authorization and access control
        public string Role { get; set; }
    }
}
