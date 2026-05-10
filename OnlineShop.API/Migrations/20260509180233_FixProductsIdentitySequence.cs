using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.API.Migrations
{
    /// <inheritdoc />
    public partial class FixProductsIdentitySequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // То же, что с Categories: serial/identity отстаёт от MAX(Id) после ручных INSERT → 23505 PK.
            migrationBuilder.Sql(
                """
                SELECT setval(
                    pg_get_serial_sequence('public."Products"', 'Id'),
                    COALESCE((SELECT MAX("Id") FROM public."Products"), 0)
                );
                SELECT setval(
                    pg_get_serial_sequence('public."Users"', 'Id'),
                    COALESCE((SELECT MAX("Id") FROM public."Users"), 0)
                );
                SELECT setval(
                    pg_get_serial_sequence('public."Orders"', 'Id'),
                    COALESCE((SELECT MAX("Id") FROM public."Orders"), 0)
                );
                SELECT setval(
                    pg_get_serial_sequence('public."OrderItems"', 'Id'),
                    COALESCE((SELECT MAX("Id") FROM public."OrderItems"), 0)
                );
                SELECT setval(
                    pg_get_serial_sequence('public."CartItems"', 'Id'),
                    COALESCE((SELECT MAX("Id") FROM public."CartItems"), 0)
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
