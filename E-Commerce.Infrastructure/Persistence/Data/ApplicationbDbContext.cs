/*
 * Application Database Context
 * 
 * Purpose:
 * - Serves as the main database context for the application
 * - Inherits from IdentityDbContext to support ASP.NET Core Identity
 * - Configures entity relationships and database schema
 * 
 * Key Concepts:
 * - Entity Framework Core: ORM for database operations
 * - Code-First Approach: Entities define the database schema
 * - Fluent API: Used for detailed entity configuration
 * 
 * Features:
 * - Identity Integration: Built-in user management capabilities
 * - Entity Relationships: Defines relationships between domain models
 * - Cascade Delete: Configures deletion behavior for related entities
 */

using E_Commerce.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Persistence.Data
{
    public class ApplicationbDbContext : IdentityDbContext
    {
        // Constructor accepting DbContext options for configuration
        public ApplicationbDbContext(DbContextOptions<ApplicationbDbContext> options) : base(options)
        {
        }

        #region Entity DbSet Definitions
        // DbSet properties represent database tables
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        #endregion

        #region Entity Configuration
        // Configure entity relationships using Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call base configuration for Identity tables
            base.OnModelCreating(modelBuilder);

            #region Shopping Cart Relationships
            // Configure Cart Header - User relationship (1:Many)
            modelBuilder.Entity<CartHeader>()
                .HasOne(c => c.User)                    // CartHeader has one User
                .WithMany(u => u.CartHeaders)           // User has many CartHeaders
                .HasForeignKey(c => c.UserId)          // Foreign key property
                .OnDelete(DeleteBehavior.Cascade);     // Delete cart when user is deleted

            // Configure Cart Details - Cart Header relationship (1:Many)
            modelBuilder.Entity<CartDetails>()
                .HasOne(c => c.CartHeader)
                .WithMany(ch => ch.CartDetails)
                .HasForeignKey(c => c.CartHeaderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Cart Details - Product relationship (1:Many)
            modelBuilder.Entity<CartDetails>()
                .HasOne(c => c.Product)
                .WithMany(p => p.CartDetails)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Restrict);    // Prevent product deletion if in cart
            #endregion

            #region Order Relationships
            // Configure Order Header - User relationship (1:Many)
            modelBuilder.Entity<OrderHeader>()
                .HasOne(o => o.User)
                .WithMany(u => u.OrderHeaders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Order Details - Order Header relationship (1:Many)
            modelBuilder.Entity<OrderDetails>()
                .HasOne(o => o.OrderHeader)
                .WithMany(oh => oh.OrderDetails)
                .HasForeignKey(o => o.OrderHeaderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Order Details - Product relationship (1:Many)
            modelBuilder.Entity<OrderDetails>()
                .HasOne(o => o.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict);    // Prevent product deletion if in order
            #endregion
        }
        #endregion
    }
}
