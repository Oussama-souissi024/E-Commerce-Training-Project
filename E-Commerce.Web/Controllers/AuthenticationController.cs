using E_Commerce.Core.DTOs;
using E_Commerce.Core.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Data;
using E_Commerce.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthService _authService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AuthenticationController(IAuthService authService, SignInManager<ApplicationUser> signInManager)
        {
           _authService = authService;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text="RoleAdmin",Value="Admin"},
                new SelectListItem{Text="RoleCustomer",Value="Customer"},
            };

            ViewBag.RoleList = roleList;
            return View();
        }
        // Handle registration form submission
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto registrationRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(registrationRequest);
            }

            var result = await _authService.Register(registrationRequest);
            if (string.IsNullOrEmpty(result)) // Assuming empty result indicates success
            {
                // Redirect to login page after successful registration
                TempData["success"] = "Registration Successful";
                return RedirectToAction("Login");
            }

            TempData["error"] = "Model Invalid";
            ModelState.AddModelError(string.Empty, result);
            return View(registrationRequest);
        }

        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new();
            return View(loginRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginRequestDto);
            }

            var user = await _authService.Login(loginRequestDto);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginRequestDto.Password, false, false);
                if (result.Succeeded)
                {
                    // Redirect to the home page or a dashboard after successful login
                    TempData["success"] = "Login Successful";
                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["error"] = "Model Invalid";
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(loginRequestDto);
        }

        // Handle user logout
        public async Task<IActionResult> Logout()
        {
            // Déconnecter l'utilisateur
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
     
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
