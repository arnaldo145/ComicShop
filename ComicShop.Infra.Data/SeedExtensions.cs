using ComicShop.Infra.Helpers;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComicShop.Infra.Data
{
    public static class SeedExtensions
    {
        public static void SeedUser(this MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"INSERT INTO [dbo].[users] (Id, Type, Name, Email,Password) 
                                    VALUES (NEWID(), 2,'UserAdmin','admin@admin.com','{EncryptionHelper.Encrypt("admin")}')");
        }
    }
}
