using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComicShop.Infra.Data.Migrations
{
    public partial class AddAmountAndReleaseDateComicBook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "ComicBooks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "ComicBooks",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "ComicBooks");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "ComicBooks");
        }
    }
}
