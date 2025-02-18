using Azure;
using E_Commerce.Core.DTOs;
using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;
using E_Commerce.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Stripe.Climate;
using System.Security.Claims;

namespace E_Commerce.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICart _cartService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOrder _orderService;
        private readonly IStripeSession _stripeSession;
        private readonly IConfiguration _configuration;

        public CartController(ICart cartService,
                             SignInManager<ApplicationUser> signInManager,
                             IOrder orderService,
                             IStripeSession stripeSession,
                             IConfiguration configuration)
        {
            _cartService = cartService;
            _signInManager = signInManager;
            _orderService = orderService;
            _stripeSession = stripeSession;
            _configuration = configuration;
        }
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }
        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            // Retrieve the user ID from the SignInManager
            var user = await _signInManager.UserManager.GetUserAsync(User);

            if (string.IsNullOrEmpty(user.Id))
            {
                // Return an empty cart if no user ID is found in the cookie
                return new CartDto();
            }

            // Call the cart service to fetch the cart for the logged-in user
            CartDto? cart = await _cartService.GetCartByIdsync(user.Id);

            if (cart == null)
            {
                return new CartDto();
            }

            // Return an empty cart if the service response is not successful
            return cart;
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {

            var response = await _cartService.ApplyCouponAsync(cartDto);
            if (response == "")
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            TempData["error"] = response; ;
            return RedirectToAction(nameof(CartIndex));
        }
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = "";
            var response = await _cartService.ApplyCouponAsync(cartDto);
            if (response == "")
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            TempData["error"] = response;
            return RedirectToAction(nameof(CartIndex));
        }
        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var response = await _cartService.RemoveCart(cartDetailsId);
            if (response == "")
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }
        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            try
            {
                CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
                cart.CartHeader.Phone = cartDto.CartHeader.Phone;
                cart.CartHeader.Email = cartDto.CartHeader.Email;
                cart.CartHeader.Name = cartDto.CartHeader.Name;

                var order = await _orderService.CreateOrderAsync(cart);

                if (order != null)
                {
                    // Create Stripe session
                    var domain = $"{Request.Scheme}://{Request.Host.Value}/";
                    var stripeRequestDto = new StripeRequestDto()
                    {
                        OrderHeader = order,
                        ApprovedUrl = domain + "cart/Confirmation?orderId=" + order.OrderHeaderId,
                        CancelUrl = domain + "cart/Checkout"
                    };

                    var stripeResponse = await _stripeSession.CreateStripeSessionAsync(stripeRequestDto);
                    if (stripeResponse != null && !string.IsNullOrEmpty(stripeResponse.StripeSessionUrl))
                    {
                        // Store the Stripe session ID in TempData for later validation
                        TempData["StripeSessionId"] = stripeResponse.StripeSessionId;

                        // Redirect to Stripe checkout
                        return Redirect(stripeResponse.StripeSessionUrl);
                    }

                    TempData["error"] = "Error processing payment. Please try again.";
                    return View(cart);
                }

                TempData["error"] = "Could not create your order. Please try again.";
            }
            catch (Exception ex)
            {
                TempData["error"] = "An unexpected error occurred. Please try again.";
                // Log the error
                Console.Error.WriteLine($"Checkout error: {ex.Message}");
            }

            return View(await LoadCartDtoBasedOnLoggedInUser());
        }
        public async Task<IActionResult> Confirmation(int orderId)
        {
            try
            {
                // Validate the Stripe session and get order status
                var orderHeaderDto = await _stripeSession.ValidateStripeSession(orderId);

                if (orderHeaderDto == null)
                {
                    TempData["error"] = "Could not validate payment status. Please contact support.";
                    return RedirectToAction("Index", "Home");
                }

                switch (orderHeaderDto.Status?.ToLower())
                {
                    case "approved":
                        // Clear the cart after successful payment
                        var user = await _signInManager.UserManager.GetUserAsync(User);
                        if (user != null)
                        {
                            await _cartService.ClearCart(user.Id);
                        }
                        TempData["success"] = "Payment successful! Your order has been confirmed.";
                        break;

                    case "pending":
                        TempData["warning"] = "Your payment is being processed. We'll update you once it's confirmed.";
                        break;

                    case "payment required":
                        TempData["error"] = "Payment is required to complete your order.";
                        return RedirectToAction(nameof(Checkout));

                    case "additional action required":
                        TempData["warning"] = "Additional verification is needed for your payment.";
                        return RedirectToAction(nameof(Checkout));

                    default:
                        if (orderHeaderDto.Status?.StartsWith("Error:") == true || 
                            orderHeaderDto.Status?.StartsWith("Stripe Error:") == true)
                        {
                            TempData["error"] = orderHeaderDto.Status;
                        }
                        else
                        {
                            TempData["error"] = "Payment failed. Please try again.";
                        }
                        return RedirectToAction(nameof(Checkout));
                }

                return View(orderId);
            }
            catch (Exception ex)
            {
                TempData["error"] = "An unexpected error occurred. Please contact support.";
                // Log the error
                Console.Error.WriteLine($"Confirmation error: {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
