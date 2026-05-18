using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCategorySellerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_SellerId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_SellerId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "Categories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SellerId",
                table: "Categories",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_SellerId",
                table: "Categories",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_SellerId",
                table: "Categories",
                column: "SellerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
