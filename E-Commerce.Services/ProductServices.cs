using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;
using E_Commerce.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    // Service class for handling product-related operations
    public class ProductServices : IProduct
    {
        private readonly IRepository<Product> _repository; // Repository for generic CRUD operations
        private readonly ApplicationbDbContext _context;   // Database context for direct queries

        public ProductServices(IRepository<Product> repository, ApplicationbDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        // Add a new product to the database
        public async Task<Product> AddAsync(Product product)
        {
            var addPoduct = await _repository.AddAsync(product); // Add product using repository
            _repository.SaveChanges(); // Save changes to the database
            return addPoduct; // Return the added product
        }

        // Retrieve a product by its ID
        public async Task<Product> ReadByIdAsync(int id)
        {
            return await _context.Products
                                 .Include(p => p.Category) // Inclure la navigation vers Category
                                 .FirstOrDefaultAsync(p => p.ProductID == id);
        }

        // Retrieve all products without navigation properties
        // Retrieve all products without navigation properties
        public async Task<IEnumerable<Product>> ReadAllAsync()
        {
            return await _repository.ReadAll(); // Return all products from the repository
        }

        // Retrieve all products including their associated categories
        public async Task<List<Product>> ReadAllWithLinkAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync(); // Use Include to fetch related categories
        }

        // Update an existing product
        public void Update(Product product)
        {
            _repository.Update(product); // Update the product using the repository
            _repository.SaveChanges(); // Save changes to the database
        }

        // Delete a product by its ID
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id); // Delete the product using the repository
            _repository.SaveChanges(); // Save changes to the database
        }
    }
}
