using Microsoft.EntityFrameworkCore.Migrations;

namespace UsersAndRewards.Migrations
{
    public partial class AddPhotoAndImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Rewards",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Rewards");
        }
    }
}
