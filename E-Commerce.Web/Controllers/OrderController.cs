using E_Commerce.Core.DTOs;
using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;
using E_Commerce.Core.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe.Climate;
using System.IdentityModel.Tokens.Jwt;

namespace E_Commerce.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrder _orderService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IStripeSession _stripeSession;
        public OrderController(IOrder orderService, 
                               SignInManager<ApplicationUser> signInManager,
                               IStripeSession stripeSession)
        {
            _orderService = orderService;
            _signInManager = signInManager;
            _stripeSession = stripeSession;

        }
        [Authorize]
        public IActionResult OrderIndex()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            // Retrieve the user ID from the SignInManager
            var user = await _signInManager.UserManager.GetUserAsync(User);

            var responseOrder = await _orderService.GetOrderAsync(orderId);          
            
            if (responseOrder != null)
            {
                if (!User.IsInRole(StaticDetails.RoleAdmin) && user.Id != responseOrder.UserId)
                {
                    TempData["error"] = "Order details not found";
                    return RedirectToAction("Index", "Home");
                }
                return View(responseOrder);
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            try
            {
                IEnumerable<OrderHeaderDto> list;
                
                // Get all orders based on user role
                list = await _orderService.GetAllOrderAsync(User);
                
                if (list != null)
                {
                    // Filter based on status if provided
                    switch (status?.ToLower())
                    {
                        case "approved":
                            list = list.Where(u => u.Status == StaticDetails.Status_Approved);
                            break;
                        case "readyforpickup":
                            list = list.Where(u => u.Status == StaticDetails.Status_ReadyForPickup);
                            break;
                        case "cancelled":
                            list = list.Where(u => u.Status == StaticDetails.Status_Cancelled || 
                                                 u.Status == StaticDetails.Status_Refunded);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    list = Enumerable.Empty<OrderHeaderDto>();
                }

                return Json(new { data = list.OrderByDescending(u => u.OrderHeaderId) });
            }
            catch (Exception ex)
            {
                // Return empty list in case of error
                return Json(new { data = Enumerable.Empty<OrderHeaderDto>() });
            }
        }

        [HttpPost("OrderReadyForPickup")]
        [Authorize(Roles = StaticDetails.RoleAdmin)]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderAsync(orderId);
                if (order == null)
                {
                    TempData["error"] = "Order not found";
                    return RedirectToAction(nameof(OrderDetail), new { orderId });
                }

                var response = await _orderService.UpdateOrderStatusAsync(orderId,StaticDetails.Status_ReadyForPickup);
                if (response == StaticDetails.Status_ReadyForPickup)
                {
                    TempData["success"] = "Status updated successfully";
                    return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while updating the order status";
                return RedirectToAction(nameof(OrderDetail), new { orderId });
            }
        }

        [HttpPost("CompleteOrder")]
        [Authorize(Roles = StaticDetails.RoleAdmin)]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderAsync(orderId);
                if (order == null)
                {
                    TempData["error"] = "Order not found";
                    return RedirectToAction(nameof(OrderDetail), new { orderId });
                }

                var response = await _orderService.UpdateOrderStatusAsync(orderId, StaticDetails.Status_Completed);
                if (response == StaticDetails.Status_Completed)
                {
                    TempData["success"] = "Status updated successfully";
                    return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while updating the order status";
                return RedirectToAction(nameof(OrderDetail), new { orderId });
            }
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderAsync(orderId);
                if (order == null)
                {
                    TempData["error"] = "Order not found";
                    return RedirectToAction(nameof(OrderDetail), new { orderId });
                }

                // Only allow cancellation of approved orders
                if (order.Status == StaticDetails.Status_Approved)
                {
                    if (!string.IsNullOrEmpty(order.PaymentIntentId))
                    {
                        var refundResult = await _stripeSession.StripeRefundOptions(order.PaymentIntentId);
                        if (!refundResult)
                        {
                            TempData["error"] = "Failed to process refund";
                            return RedirectToAction(nameof(OrderDetail), new { orderId });
                        }
                    }

                    var response = await _orderService.UpdateOrderStatusAsync(orderId, StaticDetails.Status_Cancelled);
                    if (response != null)
                    {
                        TempData["success"] = "Order cancelled and refunded successfully";
                    }
                    else
                    {
                        TempData["error"] = "Failed to update order status";
                    }
                }
                else
                {
                    TempData["error"] = "Order cannot be cancelled in its current status";
                }

                return RedirectToAction(nameof(OrderDetail), new { orderId });
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while cancelling the order";
                return RedirectToAction(nameof(OrderDetail), new { orderId });
            }
        }
    }
}
