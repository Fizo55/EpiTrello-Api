using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EpiTrello.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class blockdescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long[]>(
                name: "UserIds",
                table: "Boards",
                type: "bigint[]",
                nullable: true,
                oldClrType: typeof(long[]),
                oldType: "bigint[]");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Blocks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Blocks");

            migrationBuilder.AlterColumn<long[]>(
                name: "UserIds",
                table: "Boards",
                type: "bigint[]",
                nullable: false,
                defaultValue: new long[0],
                oldClrType: typeof(long[]),
                oldType: "bigint[]",
                oldNullable: true);
        }
    }
}
