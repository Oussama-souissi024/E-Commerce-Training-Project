/*
 * Infrastructure Registration Module
 * 
 * Purpose:
 * - This static class serves as a central configuration point for all infrastructure-related dependencies
 * - It follows the Extension Method pattern to extend WebApplicationBuilder functionality
 * - Implements the Dependency Injection pattern for loose coupling between layers
 * 
 * Key Concepts:
 * - Extension Methods: Allows adding methods to existing types without modifying them
 * - Dependency Injection: Helps in managing dependencies and promoting loose coupling
 * - Generic Repository Pattern: Provides a standard interface for data operations
 */

using E_Commerce.Core.Interfaces;
using E_Commerce.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace E_Commerce.Infrastructure
{
    public static class InfrastructureRegistration
    {
        // Extension method for WebApplicationBuilder to register infrastructure services
        public static void AddInfrastructureRegistraion(this WebApplicationBuilder builder)
        {
            // Register the generic repository with transient lifetime
            // Transient: New instance created for each request
            builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}
