using AutoMapper;
using E_Commerce.Core.Interfaces;
using E_Commerce.Services;
using E_Commerce.Core.DTOs;
using E_Commerce.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using E_Commerce.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProduct _productService;
        private readonly IMapper _mapper;
        private readonly ICart _cartService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(ILogger<HomeController> logger,
                              IProduct productService,
                              IMapper mapper,
                              ICart cartService,
                              SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _productService = productService;
            _mapper = mapper;
            _cartService = cartService;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            if(!User.Identity.IsAuthenticated)
            {
                TempData["error"] = "Please Login First";
                return  RedirectToAction("Login", "Authentication");
            }
            var productList = await _productService.ReadAllWithLinkAsync(); // Fetch all products
            if (productList == null || !productList.Any())
            {
                TempData["error"] = "There are no products";
                return View(new List<ProductViewModelDto>()); // Return an empty view model list
            }

            var productViewModelDtoList = _mapper.Map<List<ProductViewModelDto>>(productList); // Map to a list
            foreach (var productDto in productViewModelDtoList)
            {
                var correspondingProduct = productList.First(p => p.ProductID == productDto.ProductID);
                productDto.CategoryName = correspondingProduct.Category?.Name; // Set the category name
            }

            return View(productViewModelDtoList); // Pass the list to the view
        }


        public async Task<IActionResult> ProductDetails(int productId)
        {
            try
            {
                var product = await _productService.ReadByIdAsync(productId); // Fetch product by ID
                if (product == null)
                {
                    TempData["error"] = "Product not found.";
                    return RedirectToAction(nameof(Index));
                }

                var productDto = _mapper.Map<ProductViewModelDto>(product); // Map product to DTO
                productDto.CategoryName = product.Category?.Name; // Set category name
                return View(productDto); // Pass the product data to the view
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductViewModelDto productViewModelDto)
        {
            try
            {
                // Retrieve the user ID from the SignInManager
                var user = await _signInManager.UserManager.GetUserAsync(User);

                if (string.IsNullOrEmpty(user.Id))
                {
                    TempData["error"] = "User not authenticated.";
                    return RedirectToAction("Login", "Authentication");
                }

                // Create a new CartDto instance
                CartDto cartDto = new CartDto()
                {
                    CartHeader = new CartHeaderDto
                    {
                        UserId = user.Id
                    }
                };

                // Add cart details
                CartDetailsDto cartDetails = new CartDetailsDto()
                {
                    Count = productViewModelDto.Count,
                    ProductId = productViewModelDto.ProductID,
                };

                List<CartDetailsDto> cartDetailsDtos = new() { cartDetails };
                cartDto.CartDetails = cartDetailsDtos;

                // Call the service to upsert the cart
                string response = await _cartService.UpsertCartAsync(cartDto);

                if (response == "")
                {
                    TempData["success"] = "Item has been added to the Shopping Cart";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "Failed to add item to the cart. Please try again.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception if logging is set up
                // e.g., _logger.LogError(ex, "An error occurred while adding item to cart.");

                TempData["error"] = ex.Message;
                Console.WriteLine($"Error in ProductDetails: {ex.Message}");
            }

            // Return the same view with the provided product details in case of error
            return View(productViewModelDto);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
