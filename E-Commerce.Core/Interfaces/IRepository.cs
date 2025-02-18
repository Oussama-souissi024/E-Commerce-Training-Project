using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Interfaces
{
    // Generic repository interface defining basic CRUD operations
    // TEntity represents any entity class (e.g., Product, Category, Order)
    // Provides a consistent way to interact with any database entity
    public interface IRepository<TEntity> where TEntity : class
    {
        // Adds a new entity to the database asynchronously
        // Returns the added entity with its generated ID
        Task<TEntity> AddAsync(TEntity entity);

        // Retrieves a single entity by its ID asynchronously
        // Returns null if no entity is found with the given ID
        Task<TEntity> ReadById(int EntityID);

        // Retrieves all entities of type TEntity from the database
        // Returns an empty collection if no entities exist
        Task<IEnumerable<TEntity>> ReadAll();

        // Updates an existing entity in the database
        // Entity must exist and have a valid ID
        void Update(TEntity entity);

        // Deletes an entity with the specified ID asynchronously
        // Should handle cases where entity doesn't exist
        Task DeleteAsync(int EntityID);

        // Saves all pending changes to the database
        // Returns number of entities modified
        int SaveChanges();
    }
}
