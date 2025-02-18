using AutoMapper;
using Azure;
using E_Commerce.Core.DTOs;
using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;
using E_Commerce.Core.Utility;
using E_Commerce.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services
{
    public class OrderServices : IOrder
    {
        private readonly ApplicationbDbContext _db;
        private IMapper _mapper;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public OrderServices(ApplicationbDbContext db,
                             IMapper mapper,
                             SignInManager<ApplicationUser> signInManager)
        {
            _db = db;
            _mapper = mapper;
            _signInManager = signInManager;
        }

        public async Task<OrderHeaderDto?> CreateOrderAsync([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = StaticDetails.Status_Pending;
                
                var orderDetails = cartDto.CartDetails.Select(cd => new OrderDetailsDto
                {
                    ProductId = cd.ProductId,
                    Count = cd.Count,
                    Price = cd.Price,
                    ProductName = cd.Product.Name
                });
                
                orderHeaderDto.OrderDetails = orderDetails;
                orderHeaderDto.OrderTotal = Math.Round(orderHeaderDto.OrderTotal, 2);

                OrderHeader orderHeader = _mapper.Map<OrderHeader>(orderHeaderDto);

                _db.OrderHeaders.Add(orderHeader);
                await _db.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderHeader.OrderHeaderId;
                return orderHeaderDto;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in CreateOrderAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<OrderHeaderDto>> GetAllOrderAsync(ClaimsPrincipal claimUser)
        {
            try
            {
                var user = await _signInManager.UserManager.GetUserAsync(claimUser);
                if (user == null)
                {
                    throw new UnauthorizedAccessException("User not found");
                }

                IEnumerable<OrderHeader> objList;
                if (await _signInManager.UserManager.IsInRoleAsync(user, StaticDetails.RoleAdmin))
                {
                    objList = await _db.OrderHeaders
                        .Include(u => u.OrderDetails)
                        .ThenInclude(u => u.Product)
                        .OrderByDescending(u => u.OrderHeaderId)
                        .ToListAsync();
                }
                else
                {
                    objList = await _db.OrderHeaders
                        .Include(u => u.OrderDetails)
                        .ThenInclude(u => u.Product)
                        .Where(u => u.UserId == user.Id)
                        .OrderByDescending(u => u.OrderHeaderId)
                        .ToListAsync();
                }

                return _mapper.Map<IEnumerable<OrderHeaderDto>>(objList);
            }
            catch (Exception ex) 
            {
                throw new ApplicationException($"Error retrieving orders: {ex.Message}", ex);
            }
        }

        public async Task<OrderHeaderDto?> GetOrderAsync(int orderId)
        {
            try
            {
                var order = await _db.OrderHeaders
                    .Include(u => u.OrderDetails)
                    .FirstOrDefaultAsync(u => u.OrderHeaderId == orderId);

                return order != null ? _mapper.Map<OrderHeaderDto>(order) : null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving order {orderId}: {ex.Message}", ex);
            }
        }

        public async Task<string?> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            try
            {
                var order = await _db.OrderHeaders.FirstOrDefaultAsync(u => u.OrderHeaderId == orderId);
                if (order == null)
                {
                    return null;
                }

                order.Status = newStatus;
                await _db.SaveChangesAsync();
                return newStatus;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating order status for order {orderId}: {ex.Message}", ex);
            }
        }
    }
}
