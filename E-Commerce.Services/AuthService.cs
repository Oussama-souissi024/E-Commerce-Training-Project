using E_Commerce.Core.DTOs;
using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;
using E_Commerce.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace E_Commerce.Services
{
    // Authentication and Authorization Service implementation
    public class AuthService : IAuthService
    {
        private readonly ApplicationbDbContext _db; // Database context for interacting with the database
        private readonly UserManager<ApplicationUser> _userManager; // Manages user-related operations
        private readonly RoleManager<IdentityRole> _roleManager; // Manages role-related operations
        private readonly ILogger<AuthService> _logger; // // Logger for logging errors and information

        // Constructor to initialize dependencies
        public AuthService(RoleManager<IdentityRole> roleManager,
                           UserManager<ApplicationUser> userManager,
                           ApplicationbDbContext db,
                           ILogger<AuthService> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
            _logger = logger;
        }

        // Method to register a new user
        public async Task<string> Register([FromBody] RegistrationRequestDto registrationRequestDto)
        {
            // Create a new ApplicationUser instance with the provided details
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };

            try
            {
                // Create the user in the database using the UserManager
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);

                // If user creation is successful
                if (result.Succeeded)
                {
                    // Retrieve the created user from the database
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);

                    // Return success message or empty string (customize as needed)
                    if (!await _roleManager.RoleExistsAsync(registrationRequestDto.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(registrationRequestDto.Role));
                    }

                    await _userManager.AddToRoleAsync(user, registrationRequestDto.Role);
                    return "";
                }
                else
                {
                    // Return the first error description if user creation fails
                    return result.Errors.FirstOrDefault()?.Description;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the registration process for user: {Email}", registrationRequestDto.Email);
                return $"An error occurred: {ex.Message}";
            }

            // Return a generic error message in case of failure
            return "Error Encountered";
        }

        // Method to log in a user
        public async Task<ApplicationUser> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            // Find the user in the database by username (case-insensitive)
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

            // Ensure the user exists and validate the password
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequestDto.Password))
            {
                // Return null or a response indicating failure if user/password is invalid
                return null;
            }

            // Get the roles for the user
            var roles = await _userManager.GetRolesAsync(user);

            return user;
        }

        // Method to assign a role to a user
        public async Task<bool> AssignRole(string email, string roleName)
        {
            // Find the user in the database by email (case-insensitive)
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

            if (user != null)
            {
                // Check if the role exists, and create it if it does not
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }

                // Assign the role to the user
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }

            // Return false if the user is not found
            return false;
        }

    }
}
