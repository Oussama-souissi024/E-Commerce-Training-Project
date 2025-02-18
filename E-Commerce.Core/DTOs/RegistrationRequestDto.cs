using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.DTOs
{
    // DTO (Data Transfer Object) for handling user registration requests
    // Contains all necessary information to create a new user account
    public class RegistrationRequestDto
    {
        // User's email address, will be used for login and communication
        public string Email { get; set; }

        // User's display name in the application
        public string Name { get; set; }

        // Contact number for order updates and communication
        public string PhoneNumber { get; set; }

        // User's chosen password (will be hashed before storage)
        public string Password { get; set; }

        // Optional role assignment (e.g., "Customer" or "Admin")
        // Null means default role (usually "Customer")
        public string? Role { get; set; }
    }
}
