using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerce.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchemaToEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Entity");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "Entity");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Products",
                newSchema: "Entity");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "Orders",
                newSchema: "Entity");

            migrationBuilder.RenameTable(
                name: "OrderItems",
                newName: "OrderItems",
                newSchema: "Entity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Users",
                schema: "Entity",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Products",
                schema: "Entity",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "Orders",
                schema: "Entity",
                newName: "Orders");

            migrationBuilder.RenameTable(
                name: "OrderItems",
                schema: "Entity",
                newName: "OrderItems");
        }
    }
}
