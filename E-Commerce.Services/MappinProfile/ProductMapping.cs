/*
 * Product AutoMapper Profile
 * 
 * Purpose:
 * - Handles mapping between Product entities and DTOs
 * - Ensures consistent data transformation
 * - Supports product-related operations across layers
 * 
 * Key Concepts:
 * - AutoMapper Configuration: Type conversion setup
 * - Data Transfer Objects: API contract management
 * - Clean Architecture: Separation of concerns
 */

using AutoMapper;
using E_Commerce.Core.DTOs;
using E_Commerce.Core.Entities;

namespace E_Commerce.Services.MappinProfile
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            // Configure Product entity to DTO mapping
            // ReverseMap enables automatic two-way conversion
            // This includes all properties defined in both types
            CreateMap<Product, ProductDto>().ReverseMap();

            // Configure Product to ProductViewModelDto mapping
            CreateMap<Product, ProductViewModelDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ReverseMap()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.CartDetails, opt => opt.Ignore());
        }
    }
}
