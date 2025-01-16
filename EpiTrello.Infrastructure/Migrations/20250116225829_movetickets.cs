using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EpiTrello.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class movetickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Blocks_BlockId",
                table: "Tickets");

            migrationBuilder.AlterColumn<long>(
                name: "BlockId",
                table: "Tickets",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "BoardId",
                table: "Tickets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_BoardId",
                table: "Tickets",
                column: "BoardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Blocks_BlockId",
                table: "Tickets",
                column: "BlockId",
                principalTable: "Blocks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Boards_BoardId",
                table: "Tickets",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Blocks_BlockId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Boards_BoardId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_BoardId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "BoardId",
                table: "Tickets");

            migrationBuilder.AlterColumn<long>(
                name: "BlockId",
                table: "Tickets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Blocks_BlockId",
                table: "Tickets",
                column: "BlockId",
                principalTable: "Blocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
