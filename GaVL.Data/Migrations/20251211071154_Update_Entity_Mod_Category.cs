using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaVL.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Entity_Mod_Category : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "roles",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "mods",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "categories",
                newName: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "roles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "mods",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "categories",
                newName: "Id");
        }
    }
}
