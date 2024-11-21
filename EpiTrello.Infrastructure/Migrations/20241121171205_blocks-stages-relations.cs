using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EpiTrello.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class blocksstagesrelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Block_Boards_BoardId",
                table: "Block");

            migrationBuilder.DropForeignKey(
                name: "FK_Stage_Boards_BoardId",
                table: "Stage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stage",
                table: "Stage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Block",
                table: "Block");

            migrationBuilder.RenameTable(
                name: "Stage",
                newName: "Stages");

            migrationBuilder.RenameTable(
                name: "Block",
                newName: "Blocks");

            migrationBuilder.RenameIndex(
                name: "IX_Stage_BoardId",
                table: "Stages",
                newName: "IX_Stages_BoardId");

            migrationBuilder.RenameIndex(
                name: "IX_Block_BoardId",
                table: "Blocks",
                newName: "IX_Blocks_BoardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stages",
                table: "Stages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blocks",
                table: "Blocks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_Id",
                table: "Stages",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_Id",
                table: "Blocks",
                column: "Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Blocks_Boards_BoardId",
                table: "Blocks",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stages_Boards_BoardId",
                table: "Stages",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blocks_Boards_BoardId",
                table: "Blocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Stages_Boards_BoardId",
                table: "Stages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stages",
                table: "Stages");

            migrationBuilder.DropIndex(
                name: "IX_Stages_Id",
                table: "Stages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Blocks",
                table: "Blocks");

            migrationBuilder.DropIndex(
                name: "IX_Blocks_Id",
                table: "Blocks");

            migrationBuilder.RenameTable(
                name: "Stages",
                newName: "Stage");

            migrationBuilder.RenameTable(
                name: "Blocks",
                newName: "Block");

            migrationBuilder.RenameIndex(
                name: "IX_Stages_BoardId",
                table: "Stage",
                newName: "IX_Stage_BoardId");

            migrationBuilder.RenameIndex(
                name: "IX_Blocks_BoardId",
                table: "Block",
                newName: "IX_Block_BoardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stage",
                table: "Stage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Block",
                table: "Block",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Block_Boards_BoardId",
                table: "Block",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stage_Boards_BoardId",
                table: "Stage",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
