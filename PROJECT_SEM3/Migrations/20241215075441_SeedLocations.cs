using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROJECT_SEM3.Migrations
{
    /// <inheritdoc />
    public partial class SeedLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
        table: "Locations",
        columns: new[] { "Id", "Country", "City" },
        values: new object[,]
        {
            { 1, "United States", "New York" },
            { 2, "United States", "Los Angeles" },
            { 3, "Canada", "Toronto" },
            { 4, "Canada", "Vancouver" },
            { 5, "United Kingdom", "London" },
            { 6, "United Kingdom", "Manchester" },
            { 7, "Vietnamese", "Hanoi" }
        });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "Locations", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "Locations", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "Locations", keyColumn: "Id", keyValue: 3);
            migrationBuilder.DeleteData(table: "Locations", keyColumn: "Id", keyValue: 4);
            migrationBuilder.DeleteData(table: "Locations", keyColumn: "Id", keyValue: 5);
            migrationBuilder.DeleteData(table: "Locations", keyColumn: "Id", keyValue: 6);
            migrationBuilder.DeleteData(table: "Locations", keyColumn: "Id", keyValue: 7);
        }
    }
}
