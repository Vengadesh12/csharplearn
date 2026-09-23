using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoleManagementBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Baseline schema: Existing tables (departments, roles, visitors, designations, users, audit_logs)
            // already exist in the database.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
