using AutoMapper;
using Azure;
using E_Commerce.Core.DTOs;
using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;
using E_Commerce.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services
{
    public class CartService : ICart
    {
        private readonly ApplicationbDbContext _db;
        private readonly IMapper _mapper;
        private readonly IProduct _productService;
        private readonly ICoupon _couponService;


        public CartService(ApplicationbDbContext db,
                           IProduct productService,
                           IMapper mapper,
                           ICoupon couponService)
        {
            _db = db;
            _mapper = mapper;
            _productService = productService;
            _couponService = couponService;
        }

        public async Task<CartDto?> GetCartByIdsync(string UserId)
        {
            try
            {
                var cartHeaderEntity = _db.CartHeaders.FirstOrDefault(u => u.UserId == UserId);
                if (cartHeaderEntity == null)
                {
                    // Handle the case where no cart exists for the given UserId
                    return null; // or create a new CartDto with default values
                }

                // Initialize the CartDto and map the CartHeader from the database
                CartDto cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_db.CartHeaders.First(u => u.UserId == UserId))

                };

                // Map the CartDetails for the specific CartHeaderId
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails
                    .Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId));

                // Retrieve all products and map them to DTOs
                IEnumerable<Product> productList = await _productService.ReadAllAsync();
                IEnumerable<ProductDto> productListDto = _mapper.Map<IEnumerable<ProductDto>>(productList);

                cart.CartHeader.CartTotal = 0;
                // Calculate the total for each cart detail
                foreach (var item in cart.CartDetails)
                {
                    item.Product = productListDto.FirstOrDefault(u => u.ProductID == item.ProductId);
                    if (item.Product != null)
                    {
                        item.Price = (double)item.Product.Price;  // Explicitly set the price from the product
                    }
                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }

                Console.WriteLine($"Cart Total: {cart.CartHeader.CartTotal}");

                // Apply a coupon if one exists
                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    Coupon coupon = await _couponService.ReadByCouponCodeAsync(cart.CartHeader.CouponCode);
                    if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }

                // Return the final cart
                return cart;
            }
            catch (Exception ex)
            {
                // Log the exception to the console (replace with a logging framework like Serilog or ILogger)
                Console.Error.WriteLine($"An error occurred in GetCartByIdsync: {ex.Message}");

                // Option 1: Rethrow the exception to allow the caller to handle it
                throw;

                // Option 2: Return a default cart (uncomment this if you choose not to throw)
                // return new CartDto
                // {
                //     CartHeader = new CartHeaderDto(),
                //     CartDetails = new List<CartDetailsDto>()
                // };

                // Option 3: Add custom error handling logic here if necessary
            }
        }

        public async Task<string?> UpsertCartAsync(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //Create CarHeader 
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //if headernot not null
                    //check if details has seem  product
                    CartDetails cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        c => c.ProductId == cartDto.CartDetails.First().ProductId &&
                        c.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        //Create CartDetails
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        //Update Count
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }   
                }
                return "";
            }
            catch (Exception ex)
            {
                return  ex.Message;
            }
        }

        public async Task<string?> ApplyCouponAsync([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();
                return "";
            }
            catch (Exception ex)
            {
                return  ex.ToString();
            }
        }

        public async Task<string?> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _db.CartDetails
                   .First(u => u.CartDetailsId == cartDetailsId);

                int totalCountofCartItem = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);
                if (totalCountofCartItem == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders
                       .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();

                return "";
            }
            catch (Exception ex)
            {
                return  ex.Message.ToString();

            }
            return null;
        }

        public async Task<string?> ClearCart(string userId)
        {
            try
            {
                // Find the cart header for the user
                var cartHeader = await _db.CartHeaders
                    .Include(ch => ch.CartDetails)  // Include cart details to remove them all at once
                    .FirstOrDefaultAsync(ch => ch.UserId == userId);

                if (cartHeader == null)
                {
                    // No cart found for this user
                    return "";
                }

                // Remove all cart details first
                if (cartHeader.CartDetails != null && cartHeader.CartDetails.Any())
                {
                    _db.CartDetails.RemoveRange(cartHeader.CartDetails);
                }

                // Remove the cart header
                _db.CartHeaders.Remove(cartHeader);

                // Save all changes
                await _db.SaveChangesAsync();

                return "";
            }
            catch (Exception ex)
            {
                return $"Error clearing cart: {ex.Message}";
            }
        }
    }
}
