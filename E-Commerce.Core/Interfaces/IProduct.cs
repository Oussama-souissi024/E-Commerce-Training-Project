using E_Commerce.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Interfaces
{
    // Interface defining operations specific to Product entity
    // Extends basic CRUD operations with product-specific functionality
    public interface IProduct
    {
        // Adds a new product to the catalog asynchronously
        // Returns the added product with its generated ID
        Task<Product> AddAsync(Product product);

        // Retrieves a specific product by its ID asynchronously
        // Returns null if product is not found
        Task<Product> ReadByIdAsync(int id);

        // Retrieves all products from the catalog
        // Returns basic product information
        Task<IEnumerable<Product>> ReadAllAsync();

        // Retrieves all products with related data (e.g., Category)
        // Uses eager loading to include navigation properties
        Task<List<Product>> ReadAllWithLinkAsync();

        // Updates an existing product's information
        // Product must exist and have a valid ID
        void Update(Product product);

        // Removes a product from the catalog by its ID
        // Should handle cases where product doesn't exist
        Task DeleteAsync(int id);
    }
}
