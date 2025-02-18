/*
 * Order AutoMapper Profile
 * 
 * Purpose:
 * - Manages complex order-related entity mappings
 * - Handles both OrderHeader and OrderDetails conversions
 * - Supports the complete order processing workflow
 * 
 * Key Concepts:
 * - Complex Object Mapping: Handles nested order structures
 * - Multiple DTO Types: Supports different view models
 * - Bidirectional Mapping: Two-way conversion support
 * 
 * Features:
 * - Order Status Tracking: Maps status enums correctly
 * - Payment Information: Handles payment-related fields
 * - User Association: Maintains order ownership data
 */

using AutoMapper;
using E_Commerce.Core.DTOs;
using E_Commerce.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services.MappinProfile
{
    public class OderMapping : Profile
    {
        public OderMapping()
        {
            // Configure OrderHeader mappings
            // Maps between entity and various DTOs
            CreateMap<OrderHeaderDto, CartHeaderDto>()
                .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal))
                .ReverseMap();

            // Configure OrderDetails mappings
            // Supports both create and view scenarios
            CreateMap<CartDetailsDto, OrderDetailsDto>()
                .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));

            CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
            CreateMap<OrderDetails, OrderDetailsDto>().ReverseMap();

            CreateMap<OrderDetailsDto, CartDetailsDto>().ReverseMap();
        }
    }
}
