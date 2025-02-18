/*
 * Cart AutoMapper Profile
 * 
 * Purpose:
 * - Configures mapping between Cart entities and DTOs
 * - Handles both CartHeader and CartDetails mappings
 * - Maintains data consistency across application layers
 * 
 * Key Concepts:
 * - Complex Object Mapping: Handles nested object structures
 * - DTO Pattern: Separates domain models from API contracts
 * - Bidirectional Mapping: Supports both serialization and deserialization
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
    public class CartMapping : Profile
    {
        public CartMapping()
        {
            // Configure CartHeader mappings
            // Enables two-way mapping between entity and DTO
            CreateMap<CartHeader, CartHeaderDto>().ReverseMap();

            // Configure CartDetails mappings
            // Maps both CartDetails entity to DTO and vice versa
            CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
        }
    }
}
