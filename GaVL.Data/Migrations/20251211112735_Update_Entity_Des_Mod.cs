using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaVL.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Entity_Des_Mod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("8c2fe7d1-0434-4515-8b53-76fd18c7ec12"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("7e5eea0f-023d-445d-a937-e8777ed516cf"));

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "mods",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("7e5eea0f-023d-445d-a937-e8777ed516cf"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("8c2fe7d1-0434-4515-8b53-76fd18c7ec12"));

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "mods",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
