using AutoMapper;
using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;
using E_Commerce.Services;
using E_Commerce.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace E_Commerce.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategory _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategory categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // Display a list of categories
        public async Task<IActionResult> CategoryIndex()
        {
            try
            {
                var categoryList = await _categoryService.ReadAllAsync();
                var categoryDtoList = _mapper.Map<List<CategoryDto>>(categoryList);
                return View(categoryDtoList);
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while loading categories.";
                return View(new List<CategoryDto>());
            }
        }

        // Create a new category
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryDto);
            }

            try
            {
                var category = _mapper.Map<Category>(categoryDto);
                var result = await _categoryService.AddAsync(category);

                if (result != null)
                {
                    TempData["success"] = "Category created successfully!";
                    return RedirectToAction(nameof(CategoryIndex));
                }
                else
                {
                    TempData["error"] = "Error creating category.";
                    return View(categoryDto);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred: {ex.Message}";
                return View(categoryDto);
            }
        }

        // Delete a category
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            try
            {
                var category = await _categoryService.ReadByIdAsync(categoryId);
                if (category == null)
                {
                    TempData["error"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }
                var categoryDto = _mapper.Map<CategoryDto>(category);
                return View(categoryDto);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(CategoryDto categoryDto)
        {
            try
            {
                await _categoryService.DeleteAsync(categoryDto.CategoryID);
                TempData["success"] = "Category deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction(nameof(CategoryIndex));
        }

        // Action to display the Edit view for a category
        public async Task<IActionResult> EditCategory(int categoryId)
        {
            try
            {
                var category = await _categoryService.ReadByIdAsync(categoryId);
                if (category == null)
                {
                    TempData["error"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }

                var categoryDto = _mapper.Map<CategoryDto>(category);
                return View(categoryDto);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Action to update a category
        [HttpPost]
        public async Task<IActionResult> EditCategory(CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryDto); // Return with validation errors
            }

            try
            {
                var category = _mapper.Map<Category>(categoryDto);
                _categoryService.Update(category);
                TempData["success"] = "Category updated successfully!";
                return RedirectToAction(nameof(CategoryIndex));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred: {ex.Message}";
                return View(categoryDto);
            }
        }

    }
}
