using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipiesAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert seed data for dish types
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
            { 1, "dessert" },
            { 2, "main dish" },
            { 3, "salad" }
                });

        }

        /// <inheritdoc />
       
            protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the seeded data
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3 });
        }
    
    }
}
