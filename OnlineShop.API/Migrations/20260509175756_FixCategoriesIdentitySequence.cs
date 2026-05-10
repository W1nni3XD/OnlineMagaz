using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.API.Migrations
{
    /// <inheritdoc />
    public partial class FixCategoriesIdentitySequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PG: после ручных INSERT с явным Id последовательность отстаёт → дубликат PK при INSERT.
            migrationBuilder.Sql("""
                SELECT setval(
                    pg_get_serial_sequence('public."Categories"', 'Id'),
                    COALESCE((SELECT MAX("Id") FROM public."Categories"), 0)
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Состояние последовательности не восстанавливаем.
        }
    }
}
