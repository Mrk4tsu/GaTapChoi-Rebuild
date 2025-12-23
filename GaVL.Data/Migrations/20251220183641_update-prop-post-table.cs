using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaVL.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatepropposttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "posts",
                newName: "title");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("b249d544-c721-4502-a172-0ca80dc8f5f2"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("9377d8eb-0056-481e-ac76-87d24ea3a976"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 20, 18, 36, 40, 681, DateTimeKind.Utc).AddTicks(7127),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2025, 12, 20, 18, 34, 42, 922, DateTimeKind.Utc).AddTicks(1291));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 20, 18, 36, 40, 681, DateTimeKind.Utc).AddTicks(6806),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2025, 12, 20, 18, 34, 42, 922, DateTimeKind.Utc).AddTicks(800));

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "posts",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "title",
                table: "posts",
                newName: "Title");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("9377d8eb-0056-481e-ac76-87d24ea3a976"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("b249d544-c721-4502-a172-0ca80dc8f5f2"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 20, 18, 34, 42, 922, DateTimeKind.Utc).AddTicks(1291),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2025, 12, 20, 18, 36, 40, 681, DateTimeKind.Utc).AddTicks(7127));

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "posts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 20, 18, 34, 42, 922, DateTimeKind.Utc).AddTicks(800),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2025, 12, 20, 18, 36, 40, 681, DateTimeKind.Utc).AddTicks(6806));
        }
    }
}
