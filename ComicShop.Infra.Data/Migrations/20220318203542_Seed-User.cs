using Microsoft.EntityFrameworkCore.Migrations;

namespace ComicShop.Infra.Data.Migrations
{
    public partial class SeedUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SeedExtensions.SeedUser(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op: legacy seed disabled.
        }
    }
}
