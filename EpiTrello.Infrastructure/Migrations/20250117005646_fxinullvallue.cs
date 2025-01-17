using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EpiTrello.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fxinullvallue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long[]>(
                name: "TicketsId",
                table: "Blocks",
                type: "bigint[]",
                nullable: true,
                oldClrType: typeof(long[]),
                oldType: "bigint[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long[]>(
                name: "TicketsId",
                table: "Blocks",
                type: "bigint[]",
                nullable: false,
                defaultValue: new long[0],
                oldClrType: typeof(long[]),
                oldType: "bigint[]",
                oldNullable: true);
        }
    }
}
