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
            migrationBuilder.Sql(@"DELETE FROM [dbo].[users]");
        }
    }
}
