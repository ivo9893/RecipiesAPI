using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RecipiesAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitsTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "RecipeIngredients");

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "RecipeIngredients",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_UnitId",
                table: "RecipeIngredients",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_Units_UnitId",
                table: "RecipeIngredients",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_Units_UnitId",
                table: "RecipeIngredients");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropIndex(
                name: "IX_RecipeIngredients_UnitId",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "RecipeIngredients");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "RecipeIngredients",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
