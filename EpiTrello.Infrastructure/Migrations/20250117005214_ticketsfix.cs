using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EpiTrello.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ticketsfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long[]>(
                name: "TicketsId",
                table: "Blocks",
                type: "bigint[]",
                nullable: false,
                defaultValue: new long[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
