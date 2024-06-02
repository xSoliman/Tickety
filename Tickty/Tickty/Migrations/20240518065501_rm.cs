using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickty.Migrations
{
    /// <inheritdoc />
    public partial class rm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatCount",
                table: "Stadiums");

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2024, 5, 18, 9, 55, 1, 486, DateTimeKind.Local).AddTicks(3247));

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 2,
                column: "Date",
                value: new DateTime(2024, 5, 19, 9, 55, 1, 486, DateTimeKind.Local).AddTicks(3303));

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 3,
                column: "Date",
                value: new DateTime(2024, 5, 20, 9, 55, 1, 486, DateTimeKind.Local).AddTicks(3308));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeatCount",
                table: "Stadiums",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2024, 5, 18, 9, 38, 38, 961, DateTimeKind.Local).AddTicks(485));

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 2,
                column: "Date",
                value: new DateTime(2024, 5, 19, 9, 38, 38, 961, DateTimeKind.Local).AddTicks(536));

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 3,
                column: "Date",
                value: new DateTime(2024, 5, 20, 9, 38, 38, 961, DateTimeKind.Local).AddTicks(540));

            migrationBuilder.UpdateData(
                table: "Stadiums",
                keyColumn: "Id",
                keyValue: 1,
                column: "SeatCount",
                value: "10000");

            migrationBuilder.UpdateData(
                table: "Stadiums",
                keyColumn: "Id",
                keyValue: 2,
                column: "SeatCount",
                value: "20000");

            migrationBuilder.UpdateData(
                table: "Stadiums",
                keyColumn: "Id",
                keyValue: 3,
                column: "SeatCount",
                value: "15000");
        }
    }
}
