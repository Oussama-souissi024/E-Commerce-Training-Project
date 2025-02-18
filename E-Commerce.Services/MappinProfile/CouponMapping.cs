/*
 * Coupon AutoMapper Profile
 * 
 * Purpose:
 * - Defines mapping configuration between Coupon entity and its DTOs
 * - Implements AutoMapper's Profile class for type conversion
 * - Ensures clean separation between domain entities and DTOs
 * 
 * Key Concepts:
 * - AutoMapper: Automatic object-to-object mapping
 * - DTO Pattern: Data Transfer Objects for API communication
 * - Bidirectional Mapping: Maps in both directions (entity ↔ DTO)
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
    public class CouponMapping : Profile
    {
        public CouponMapping()
        {
            // Map Coupon entity to CouponDto and vice versa
            // ReverseMap() enables bidirectional mapping
            CreateMap<Coupon, CouponDto>().ReverseMap();
        }
    }
}
