using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceApp.Migrations
{
    public partial class ChangingSizes2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "ApplicationProducts");

            migrationBuilder.AddColumn<bool>(
                name: "SizeL",
                table: "ApplicationProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SizeM",
                table: "ApplicationProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SizeS",
                table: "ApplicationProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SizeXL",
                table: "ApplicationProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SizeL",
                table: "ApplicationProducts");

            migrationBuilder.DropColumn(
                name: "SizeM",
                table: "ApplicationProducts");

            migrationBuilder.DropColumn(
                name: "SizeS",
                table: "ApplicationProducts");

            migrationBuilder.DropColumn(
                name: "SizeXL",
                table: "ApplicationProducts");

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "ApplicationProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
