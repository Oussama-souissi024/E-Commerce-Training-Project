using E_Commerce.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Interfaces
{
    // Interface defining operations for managing product categories
    // Provides methods for CRUD operations and category lookups
    public interface ICategory
    {
        // Adds a new category to the system
        // Returns the created category with its generated ID
        Task<Category> AddAsync(Category category);

        // Retrieves a specific category by its ID
        // Returns null if category is not found
        Task<Category> ReadByIdAsync(int id);

        // Retrieves all available categories
        // Returns empty collection if no categories exist
        Task<IEnumerable<Category>> ReadAllAsync();

        // Looks up a category's ID by its name
        // Useful for searching and filtering products
        // Returns null if category name doesn't exist
        Task<int?> GetCategoryIdByCategoryNameAsync(string categoryName);

        // Updates an existing category's information
        // Category must exist and have a valid ID
        void Update(Category category);

        // Removes a category from the system
        // Should handle cases where category has associated products
        Task DeleteAsync(int id);
    }
}
