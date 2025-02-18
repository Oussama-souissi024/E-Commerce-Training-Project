using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;
using E_Commerce.Infrastructure.Persistence;
using E_Commerce.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services
{
    public class CategoryServices : ICategory
    {
        private readonly IRepository<Category> _repository;
        private readonly ApplicationbDbContext _context;
        public CategoryServices(IRepository<Category> repository, ApplicationbDbContext context)
        {
            _context = context;
            _repository = repository;
        }

        public async Task<Category> AddAsync(Category category)
        {
            var addCategory = await _repository.AddAsync(category);
            _repository.SaveChanges();
            return addCategory;
        }

        public async Task<Category> ReadByIdAsync(int id)
        {
            return await _repository.ReadById(id);
        }

        public async Task<IEnumerable<Category>> ReadAllAsync()
        {
            return await _repository.ReadAll();
        }

        public void Update(Category category)
        {
            _repository.Update(category);
            _repository.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
            _repository.SaveChanges();
        }

        public async Task<int?> GetCategoryIdByCategoryNameAsync(string categoryName)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
            return category?.CategoryID; // Return the CategoryID or null if not found        }
        }
    }
}
