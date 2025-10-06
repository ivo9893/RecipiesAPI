using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipiesAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRowIDtoRecipies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RowID",
                table: "Recipes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowID",
                table: "Recipes");
        }
    }
}
