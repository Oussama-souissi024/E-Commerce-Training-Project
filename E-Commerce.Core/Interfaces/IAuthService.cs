using E_Commerce.Core.DTOs;
using E_Commerce.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Interfaces
{
    // Interface defining authentication and authorization operations
    // Handles user registration, login, and role management
    public interface IAuthService
    {
        // Registers a new user in the system
        // Validates and creates user account with provided details
        // Returns success/error message
        Task<string> Register(RegistrationRequestDto registrationRequestDto);

        // Authenticates a user and creates a session
        // Validates credentials and returns user information
        // Returns null if authentication fails
        Task<ApplicationUser> Login(LoginRequestDto loginRequestDto);

        // Assigns a role to a user (e.g., Admin, Customer)
        // Used for managing user permissions
        // Returns true if role assignment succeeds
        Task<bool> AssignRole(string email, string roleName);
    }
}
