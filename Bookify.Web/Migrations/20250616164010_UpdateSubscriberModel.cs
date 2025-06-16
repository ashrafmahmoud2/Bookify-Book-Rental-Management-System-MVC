using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Web.Migrations
{
    public partial class UpdateSubscriberModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                table: "Subscribers");

            migrationBuilder.DropColumn(
                name: "Governorate",
                table: "Subscribers");

            migrationBuilder.AddColumn<int>(
                name: "SelectedArea",
                table: "Subscribers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SelectedGovernorate",
                table: "Subscribers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedArea",
                table: "Subscribers");

            migrationBuilder.DropColumn(
                name: "SelectedGovernorate",
                table: "Subscribers");

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "Subscribers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Governorate",
                table: "Subscribers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
