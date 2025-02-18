using AutoMapper;
using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;
using E_Commerce.Services;
using E_Commerce.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProduct _productService; // Service to manage product-related operations
        private readonly IFileHelper _fileHelper; // Helper service for file operations
        private readonly ICategory _categoryService; // Service to manage category-related operations
        private readonly IMapper _mapper;

        public ProductController(IProduct productService,
                                 IFileHelper fileHelper,
                                 ICategory categoryServices,
                                 IMapper mapper)
        {
            _productService = productService;
            _fileHelper = fileHelper;
            _categoryService = categoryServices;
            _mapper = mapper;
        }

        // Display the product list
        public async Task<IActionResult> ProductIndex()
        {
            var productList = await _productService.ReadAllWithLinkAsync(); // Fetch all products
            productList.AsQueryable().Include(c => c.Category);
            return View(productList); // Return the product list to the view
        }

        // Load the product creation form
        public async Task<IActionResult> CreateProduct()
        {
            // Fetch all categories and prepare them as a dropdown list
            var allCategories = await _categoryService.ReadAllAsync();
            ViewBag.categories = allCategories.Select(c => new SelectListItem
            {
                Value = c.CategoryID.ToString(), // Set category ID as the value
                Text = c.Name, // Set category name as the text
            }).ToList();

            return View(); // Return the creation form view
        }

        // Handle the product creation form submission
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDto productDto)
        {
            if (!ModelState.IsValid) // Check if the submitted data is valid
            {
                return View(productDto); // Return the view with validation errors
            }
            try
            {
                // Map the DTO to a Product entity
                Product product = _mapper.Map<Product>(productDto);
                product.ImageUrl = _fileHelper.UploadFile(productDto.ImageFile, "Images"); // Upload image and get its URL

                // Add the product to the database
                await _productService.AddAsync(product);

                // Notify the user of success
                TempData["success"] = "Product created successfully!";
                return RedirectToAction(nameof(ProductIndex)); // Redirect to the product list
            }
            catch (Exception ex)
            {
                // Log the error (optional, replace with actual logging)
                TempData["error"] = $"An error occurred: {ex.Message}";
                return View(productDto); // Return the view with the original data
            }
        }

        public async Task<IActionResult> EditProduct(int productId)
        {
            var product = await _productService.ReadByIdAsync(productId); // Retrieve the product by ID
            if (product == null)
            {
                TempData["error"] = "Product not found!";
                return RedirectToAction(nameof(ProductIndex));
            }
        
            ProductViewModelDto productEditViewModelDto = _mapper.Map<ProductViewModelDto>(product);
            productEditViewModelDto.CategoryName = product.Category?.Name;

            return View(productEditViewModelDto); // Pass the product to the view
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductViewModelDto productEditViewModelDto)
        {
            if (!ModelState.IsValid)
            {
                return View(productEditViewModelDto); // Return the view with validation errors
            }

            var product = await _productService.ReadByIdAsync(productEditViewModelDto.ProductID);
            if (product == null)
            {
                TempData["error"] = "Product not found!";
                return RedirectToAction(nameof(ProductIndex));
            }

            try
            {
                // Update product properties
                product.Name = productEditViewModelDto.Name;
                product.Price = productEditViewModelDto.Price;
                product.Description = productEditViewModelDto.Description;

                var CategoryID = await _categoryService.GetCategoryIdByCategoryNameAsync(productEditViewModelDto.CategoryName);
                if(CategoryID == null)
                {
                    throw new Exception("Category not found! Please ensure the category name is correct.");
                }
                product.CategoryID = CategoryID.Value;
                // Check if a new image file is uploaded
                if (productEditViewModelDto.ImageUrl != null)
                {
                    // Delete the old file
                    if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        _fileHelper.DeleteFile(product.ImageUrl,"Images");
                    }

                    // Upload the new file
                    product.ImageUrl = _fileHelper.UploadFile(productEditViewModelDto.Image, "Images");
                }

                // Update the product in the database
                _productService.Update(product);

                TempData["success"] = "Product updated successfully!";
                return RedirectToAction(nameof(ProductIndex));
            }
            catch (Exception ex)
            {
                // Log the error (optional, replace with actual logging)
                TempData["error"] = $"An error occurred: {ex.Message}";
                return View(productEditViewModelDto); // Return the view with the original data
            }
        }

        // Delete the product
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            // Retrieve the product by ID
            var product = await _productService.ReadByIdAsync(productId);
            if (product == null)
            {
                TempData["error"] = "Product not found!";
                return RedirectToAction(nameof(ProductIndex));
            }
            ProductDto productDto = _mapper.Map<ProductDto>(product);
            return View(productDto);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(ProductDto productDto)
        {
            try
            {
                // Retrieve the product by ID
                var product = await _productService.ReadByIdAsync(productDto.ProductID);
                if (product == null)
                {
                    TempData["error"] = "Product not found!";
                    return RedirectToAction(nameof(ProductIndex));
                }

                // Delete the product's image file if it exists
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    if(!_fileHelper.DeleteFile(product.ImageUrl,"Images"))
                    {
                        // Throw an exception if the deletion fails
                        throw new Exception("Failed to delete the image file.");
                    }
                }

                // Delete the product from the database
                await _productService.DeleteAsync(product.ProductID);

                TempData["success"] = "Product deleted successfully!";
                return RedirectToAction(nameof(ProductIndex)); // Redirect to the product list
            }
            catch (Exception ex)
            {
                // Log the error (optional, replace with actual logging)
                TempData["error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction(nameof(ProductIndex)); // Return to the product list with error message
            }
        }

    }
}
