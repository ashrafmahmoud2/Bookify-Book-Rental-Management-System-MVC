using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Web.Migrations
{
    public partial class AddRentalsAndRentalCopies1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentalCopies_AspNetUsers_CreatedById",
                table: "RentalCopies");

            migrationBuilder.DropForeignKey(
                name: "FK_RentalCopies_AspNetUsers_LastUpdatedById",
                table: "RentalCopies");

            migrationBuilder.DropIndex(
                name: "IX_RentalCopies_CreatedById",
                table: "RentalCopies");

            migrationBuilder.DropIndex(
                name: "IX_RentalCopies_LastUpdatedById",
                table: "RentalCopies");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "RentalCopies");

            migrationBuilder.DropColumn(
                name: "EndData",
                table: "RentalCopies");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RentalCopies");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "RentalCopies");

            migrationBuilder.DropColumn(
                name: "LastUpdatedOn",
                table: "RentalCopies");

            migrationBuilder.RenameColumn(
                name: "PenalyPaid",
                table: "Rentals",
                newName: "PenaltyPaid");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "RentalCopies",
                newName: "EndDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PenaltyPaid",
                table: "Rentals",
                newName: "PenalyPaid");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "RentalCopies",
                newName: "CreatedOn");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "RentalCopies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndData",
                table: "RentalCopies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RentalCopies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedById",
                table: "RentalCopies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedOn",
                table: "RentalCopies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RentalCopies_CreatedById",
                table: "RentalCopies",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RentalCopies_LastUpdatedById",
                table: "RentalCopies",
                column: "LastUpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalCopies_AspNetUsers_CreatedById",
                table: "RentalCopies",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalCopies_AspNetUsers_LastUpdatedById",
                table: "RentalCopies",
                column: "LastUpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
