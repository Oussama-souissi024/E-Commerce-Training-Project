/*
 * Database Migration: Add Count to OrderDetails
 * 
 * Purpose:
 * - Updates the OrderDetails table to include a Count column
 * - Demonstrates EF Core's migration capabilities for schema evolution
 * 
 * Key Concepts:
 * - Database Migrations: Safe way to evolve database schema
 * - Up/Down Methods: Provides forward and rollback capabilities
 * - Schema Versioning: Tracks database changes over time
 */

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Infrastructure.Migrations
{
    /// <summary>
    /// Migration to add Count column to OrderDetails table
    /// Generated: 2025-01-29 15:39:11
    /// </summary>
    public partial class UpdateOrderDetails_Add_Count : Migration
    {
        // Up method: Applies the migration changes
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Count column to OrderDetails table
            migrationBuilder.AddColumn<int>(
                name: "Count",                    // Column name
                table: "OrderDetails",            // Target table
                type: "int",                      // SQL data type
                nullable: false,                  // Not nullable
                defaultValue: 0);                 // Default value if not specified
        }

        // Down method: Reverts the migration changes
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove Count column from OrderDetails table
            migrationBuilder.DropColumn(
                name: "Count",
                table: "OrderDetails");
        }
    }
}
