using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.DTOs
{
    // DTO for handling user login requests
    // Contains the minimum required information for user authentication
    public class LoginRequestDto
    {
        // Username (typically email) used for authentication
        // This will be matched against the stored user credentials
        public string UserName { get; set; }

        // User's password for authentication
        // Will be verified against the stored hashed password
        public string Password { get; set; }
    }
}
