using AutoMapper;
using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;
using E_Commerce.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace E_Commerce.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICoupon _couponServices;
        private readonly IMapper _mapper;
        private readonly IStripeSession _stripeSession;

        public CouponController(ICoupon couponServices, 
                              IMapper mapper,
                              IStripeSession stripeSession)
        {
            _couponServices = couponServices ?? throw new ArgumentNullException(nameof(couponServices));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _stripeSession = stripeSession ?? throw new ArgumentNullException(nameof(stripeSession));
        }

        public async Task<IActionResult> CouponIndex()
        {
            try
            {
                var couponList = await _couponServices.ReadAllAsync();
                return View(couponList ?? new List<Coupon>());
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while loading the coupons: {ex.Message}";
                return View(new List<Coupon>());
            }
        }

        public IActionResult CreateCoupon()
        {
            return View(new CouponDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon(CouponDto couponDto)
        {
            if (!ModelState.IsValid)
            {
                return View(couponDto);
            }

            try
            {
                // First create the coupon in Stripe
                var stripeResponse = await _stripeSession.AddStripeCoupon(couponDto);
                
                if (stripeResponse.StartsWith("Error:") || stripeResponse.StartsWith("Stripe Error:"))
                {
                    TempData["error"] = stripeResponse;
                    return View(couponDto);
                }

                // If Stripe coupon creation was successful, create in our database
                var coupon = _mapper.Map<Coupon>(couponDto);
                coupon.StripeId = stripeResponse; // Store Stripe coupon ID

                var result = await _couponServices.AddAsync(coupon);

                if (result != null)
                {
                    TempData["success"] = "Coupon created successfully!";
                    return RedirectToAction(nameof(CouponIndex));
                }
                
                TempData["error"] = "Error creating coupon in database.";
                return View(couponDto);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred: {ex.Message}";
                return View(couponDto);
            }
        }

        public async Task<IActionResult> DeleteCoupon(int couponId)
        {
            try
            {
                var coupon = await _couponServices.ReadByIdAsync(couponId);
                if (coupon == null)
                {
                    TempData["error"] = "Coupon not found.";
                    return RedirectToAction(nameof(CouponIndex));
                }
                return View(_mapper.Map<CouponDto>(coupon));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction(nameof(CouponIndex));
            }
        }

        [HttpPost]
        [ActionName("DeleteCoupon")]
        public async Task<IActionResult> DeleteCouponConfirmed(CouponDto couponDto)
        {
            try
            {
                var coupon = await _couponServices.ReadByIdAsync(couponDto.CouponId.Value);
                if (coupon == null)
                {
                    TempData["error"] = "Coupon not found.";
                    return RedirectToAction(nameof(CouponIndex));
                }

                // Map the coupon to couponDto to ensure all properties are properly set
                couponDto = _mapper.Map<CouponDto>(coupon);

                // First delete from Stripe
                var stripeResponse = await _stripeSession.DeleteStripeCoupon(couponDto);
                
                if (stripeResponse.StartsWith("Stripe Error:") || stripeResponse.StartsWith("Error:"))
                {
                    TempData["error"] = stripeResponse;
                    return RedirectToAction(nameof(CouponIndex));
                }

                // Then delete from database
                await _couponServices.DeleteAsync(couponDto.CouponId.Value);
                
                TempData["success"] = "Coupon deleted successfully!";
                return RedirectToAction(nameof(CouponIndex));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction(nameof(CouponIndex));
            }
        }
    }
}
