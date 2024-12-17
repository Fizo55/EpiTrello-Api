using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EpiTrello.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class stageplace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Place",
                table: "Stages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Place",
                table: "Stages");
        }
    }
}
