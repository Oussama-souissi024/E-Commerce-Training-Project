/*
 * Category AutoMapper Profile
 * 
 * Purpose:
 * - Maps Category domain entity to and from DTOs
 * - Simplifies data transformation between layers
 * - Provides clean separation of concerns
 * 
 * Key Concepts:
 * - AutoMapper Profile: Configuration for type conversion
 * - Bidirectional Mapping: Two-way conversion support
 * - Clean Architecture: Supports separation of domain and API layers
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
    public class CategoryMapping : Profile
    {
        public CategoryMapping()
        {
            // Configure bidirectional mapping between Category entity and DTO
            // ReverseMap allows conversion in both directions
            CreateMap<Category, CategoryDto>().ReverseMap();
        }
    }
}
