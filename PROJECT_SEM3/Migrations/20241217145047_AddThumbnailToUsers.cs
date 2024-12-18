using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROJECT_SEM3.Migrations
{
    /// <inheritdoc />
    public partial class AddThumbnailToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "AspNetUsers");
        }
    }
}
