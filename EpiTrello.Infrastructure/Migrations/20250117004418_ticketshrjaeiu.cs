using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EpiTrello.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ticketshrjaeiu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "Blocks");

            migrationBuilder.AddColumn<List<long>>(
                name: "TicketIds",
                table: "Blocks",
                type: "bigint[]",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketIds",
                table: "Blocks");

            migrationBuilder.AddColumn<long>(
                name: "TicketId",
                table: "Blocks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
