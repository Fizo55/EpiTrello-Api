using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EpiTrello.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixtickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Blocks_BlockId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_BlockId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "BlockId",
                table: "Tickets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BlockId",
                table: "Tickets",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_BlockId",
                table: "Tickets",
                column: "BlockId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Blocks_BlockId",
                table: "Tickets",
                column: "BlockId",
                principalTable: "Blocks",
                principalColumn: "Id");
        }
    }
}
