using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaVL.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_URL_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("436aa129-a8d2-4bc8-974c-a9fb96352df1"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("8c2fe7d1-0434-4515-8b53-76fd18c7ec12"));

            migrationBuilder.CreateTable(
                name: "urls",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    url_string = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    mod_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_urls", x => x.id);
                    table.ForeignKey(
                        name: "FK_urls_mods_mod_id",
                        column: x => x.mod_id,
                        principalTable: "mods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_urls_mod_id",
                table: "urls",
                column: "mod_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "urls");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("8c2fe7d1-0434-4515-8b53-76fd18c7ec12"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("436aa129-a8d2-4bc8-974c-a9fb96352df1"));
        }
    }
}
