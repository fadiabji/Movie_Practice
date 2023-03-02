using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movie_Exercise.Data.Migrations
{
    public partial class ImageFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageFile",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFile",
                table: "Movies");
        }
    }
}
