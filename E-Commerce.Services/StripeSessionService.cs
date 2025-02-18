using E_Commerce.Core.DTOs;
using E_Commerce.Core.Interfaces;
using Stripe;
using Microsoft.Extensions.Configuration;
using Stripe.Checkout;
using E_Commerce.Core.Entities;
using System;
using E_Commerce.Infrastructure.Persistence.Data;
using static Azure.Core.HttpHeader;
using E_Commerce.Core.Utility;

namespace E_Commerce.Services
{
    public class StripeSessionService : IStripeSession
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationbDbContext _db;

        public StripeSessionService(IConfiguration configuration,
                                    ApplicationbDbContext db)
        {
            _configuration = configuration;
            _db = db;
        }

        public async Task<string> AddStripeCoupon(CouponDto couponDto)
        {
            try
            {
                StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

                var options = new CouponCreateOptions
                {
                    Duration = "once",  
                    AmountOff = (long)(couponDto.DiscountAmount * 100),
                    Name = couponDto.CouponCode,
                    Currency = "usd",
                    Id = couponDto.CouponCode,
                };

                var service = new Stripe.CouponService();
                var coupon = await service.CreateAsync(options);

                if (coupon != null && coupon.Valid)
                {
                    return coupon.Id;
                }
                return "Failed to create coupon";
            }
            catch (StripeException ex)
            {
                return $"Stripe Error: {ex.StripeError.Message}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> DeleteStripeCoupon(CouponDto couponDto)
        {
            try
            {
                StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

                var service = new Stripe.CouponService();
                await service.DeleteAsync(couponDto.CouponCode);
                return "";
            }
            catch (StripeException ex)
            {
                return $"Stripe Error: {ex.StripeError.Message}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<StripeRequestDto?> CreateStripeSessionAsync(StripeRequestDto stripeRequestDto)
        {
            try
            {
                StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    PaymentMethodTypes = new List<string> { "card" }, 
                };

                var DiscountsObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon=stripeRequestDto.OrderHeader.CouponCode
                    }
                };
                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), 
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductName,
                                Description = $"Quantity: {item.Count}"
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                if (stripeRequestDto.OrderHeader.Discount > 0)
                {
                    options.Discounts = DiscountsObj;
                }

                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                stripeRequestDto.StripeSessionUrl = session.Url;
                stripeRequestDto.StripeSessionId = session.Id;

                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                await _db.SaveChangesAsync();

                return stripeRequestDto;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in CreateStripeSessionAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<OrderHeaderDto> ValidateStripeSession(int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = await _db.OrderHeaders.FindAsync(orderHeaderId)
                    ?? throw new Exception($"Order header with ID {orderHeaderId} not found");

                StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

                var sessionService = new SessionService();
                var session = await sessionService.GetAsync(orderHeader.StripeSessionId);

                if (session == null)
                {
                    throw new Exception($"Stripe session not found for ID: {orderHeader.StripeSessionId}");
                }

                if (session.PaymentStatus == "unpaid")
                {
                    return new OrderHeaderDto
                    {
                        OrderHeaderId = orderHeader.OrderHeaderId,
                        Status = "Pending",
                        StripeSessionId = orderHeader.StripeSessionId
                    };
                }

                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.GetAsync(session.PaymentIntentId);

                switch (paymentIntent.Status.ToLower())
                {
                    case "succeeded":
                        orderHeader.PaymentIntentId = paymentIntent.Id;
                        orderHeader.Status = "Approved";
                        await _db.SaveChangesAsync();
                        return new OrderHeaderDto
                        {
                            OrderHeaderId = orderHeader.OrderHeaderId,
                            Status = orderHeader.Status,
                            PaymentIntentId = orderHeader.PaymentIntentId,
                            StripeSessionId = orderHeader.StripeSessionId,
                        };

                    case "requires_payment_method":
                        orderHeader.Status = "PaymentRequired";
                        await _db.SaveChangesAsync();
                        return new OrderHeaderDto
                        {
                            OrderHeaderId = orderHeader.OrderHeaderId,
                            Status = "Payment Required",
                            StripeSessionId = orderHeader.StripeSessionId
                        };

                    case "requires_action":
                        orderHeader.Status = "PaymentPending";
                        await _db.SaveChangesAsync();
                        return new OrderHeaderDto
                        {
                            OrderHeaderId = orderHeader.OrderHeaderId,
                            Status = "Additional Action Required",
                            StripeSessionId = orderHeader.StripeSessionId
                        };

                    default:
                        orderHeader.Status = "PaymentFailed";
                        await _db.SaveChangesAsync();
                        return new OrderHeaderDto
                        {
                            OrderHeaderId = orderHeader.OrderHeaderId,
                            Status = $"Payment Failed: {paymentIntent.Status}",
                            StripeSessionId = orderHeader.StripeSessionId
                        };
                }
            }
            catch (StripeException ex)
            {
                return new OrderHeaderDto
                {
                    OrderHeaderId = orderHeaderId,
                    Status = $"Stripe Error: {ex.StripeError.Message}",
                    PaymentIntentId = null,
                    StripeSessionId = null
                };
            }
            catch (Exception ex)
            {
                return new OrderHeaderDto
                {
                    OrderHeaderId = orderHeaderId,
                    Status = $"Error: {ex.Message}",
                    PaymentIntentId = null,
                    StripeSessionId = null
                };
            }
        }

        public async Task<bool> StripeRefundOptions(string paymentIntentId)
        {
            try
            {
                StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

                var options = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntentId,
                    Reason = RefundReasons.RequestedByCustomer
                };

                var service = new RefundService();
                var refund = await service.CreateAsync(options);

                return refund.Status == "succeeded";
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
