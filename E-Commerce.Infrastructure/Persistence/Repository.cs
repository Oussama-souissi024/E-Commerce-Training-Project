/*
 * Generic Repository Implementation
 * 
 * Purpose:
 * - Provides a generic implementation of the Repository pattern
 * - Centralizes common data access operations for any entity type
 * - Abstracts the underlying data access technology (Entity Framework Core)
 * 
 * Design Patterns:
 * - Repository Pattern: Mediates between the domain and data mapping layers
 * - Generic Programming: Enables type-safe reuse of data access code
 * - Dependency Injection: DbContext is injected to maintain loose coupling
 * 
 * Key Features:
 * - Async/Await Pattern: Ensures non-blocking database operations
 * - CRUD Operations: Create, Read, Update, Delete functionality
 * - Entity Framework Integration: Leverages EF Core's DbContext and DbSet
 */

using E_Commerce.Core.Interfaces;
using E_Commerce.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Persistence
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        // Private fields to store the database context and entity set
        private readonly ApplicationbDbContext _context;
        private readonly DbSet<TEntity> _entity;

        // Constructor injection of DbContext
        public Repository(ApplicationbDbContext context)
        {
            _context = context;
            _entity = _context.Set<TEntity>();  // Get DbSet for the specific entity type
        }

        // Asynchronously adds a new entity to the database
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _entity.AddAsync(entity);
            return entity;
        }

        // Asynchronously retrieves an entity by its ID
        public async Task<TEntity> ReadById(int EntityID) =>
             await _entity.FindAsync(EntityID);

        // Asynchronously retrieves all entities of type TEntity
        public async Task<IEnumerable<TEntity>> ReadAll() =>
            await _entity.ToListAsync();

        // Updates an existing entity in the database
        public void Update(TEntity entity)
        {
            _entity.Update(entity);
        }

        // Asynchronously deletes an entity by its ID
        public async Task DeleteAsync(int EntityID)
        {
            var objectEntity = await _entity.FindAsync(EntityID);
            _entity.Remove(objectEntity);
        }

        // Persists all changes to the database
        public int SaveChanges() =>
            _context.SaveChanges();
    }
}
