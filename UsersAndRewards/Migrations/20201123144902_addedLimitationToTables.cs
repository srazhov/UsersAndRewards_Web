using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace UsersAndRewards.Migrations
{
    public partial class addedLimitationToTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                table: "Users",
                name: "Name",
                type: "nvarchar(50)",
                nullable: false);
            migrationBuilder.AlterColumn<DateTime>(
                table: "Users",
                name: "Birthdate",
                type: "date",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                table: "Rewards",
                name: "Title",
                type: "nvarchar(50)",
                nullable: false);
            migrationBuilder.AlterColumn<string>(
                table: "Rewards",
                name: "Description",
                type: "nvarchar(250)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                table: "Users",
                name: "Name",
                type: "nvarchar(max)",
                nullable: true);
            migrationBuilder.AlterColumn<DateTime>(
                table: "Users",
                name: "Birthdate",
                type: "datetime2",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                table: "Rewards",
                name: "Title",
                type: "nvarchar(max)",
                nullable: true);
            migrationBuilder.AlterColumn<string>(
                table: "Rewards",
                name: "Description",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
