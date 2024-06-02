using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickty.Migrations
{
    /// <inheritdoc />
    public partial class stad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GmapLocation",
                table: "Stadiums",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2024, 5, 19, 23, 38, 57, 183, DateTimeKind.Local).AddTicks(5125));

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 2,
                column: "Date",
                value: new DateTime(2024, 5, 20, 23, 38, 57, 183, DateTimeKind.Local).AddTicks(5182));

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 3,
                column: "Date",
                value: new DateTime(2024, 5, 21, 23, 38, 57, 183, DateTimeKind.Local).AddTicks(5187));

            migrationBuilder.UpdateData(
                table: "Stadiums",
                keyColumn: "Id",
                keyValue: 1,
                column: "GmapLocation",
                value: null);

            migrationBuilder.UpdateData(
                table: "Stadiums",
                keyColumn: "Id",
                keyValue: 2,
                column: "GmapLocation",
                value: null);

            migrationBuilder.UpdateData(
                table: "Stadiums",
                keyColumn: "Id",
                keyValue: 3,
                column: "GmapLocation",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GmapLocation",
                table: "Stadiums");

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2024, 5, 18, 21, 16, 1, 49, DateTimeKind.Local).AddTicks(1965));

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 2,
                column: "Date",
                value: new DateTime(2024, 5, 19, 21, 16, 1, 49, DateTimeKind.Local).AddTicks(2026));

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 3,
                column: "Date",
                value: new DateTime(2024, 5, 20, 21, 16, 1, 49, DateTimeKind.Local).AddTicks(2033));
        }
    }
}
