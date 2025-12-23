using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaVL.Data.Migrations
{
    /// <inheritdoc />
    public partial class addproppost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "posts",
                newName: "id");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("5ec9d72e-db34-483e-8ca3-ef743af736ff"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("b249d544-c721-4502-a172-0ca80dc8f5f2"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 20, 19, 23, 59, 155, DateTimeKind.Utc).AddTicks(9705),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2025, 12, 20, 18, 36, 40, 681, DateTimeKind.Utc).AddTicks(7127));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 20, 19, 23, 59, 155, DateTimeKind.Utc).AddTicks(9367),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2025, 12, 20, 18, 36, 40, 681, DateTimeKind.Utc).AddTicks(6806));

            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "posts",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "code",
                table: "posts");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "posts",
                newName: "Id");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("b249d544-c721-4502-a172-0ca80dc8f5f2"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("5ec9d72e-db34-483e-8ca3-ef743af736ff"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 20, 18, 36, 40, 681, DateTimeKind.Utc).AddTicks(7127),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2025, 12, 20, 19, 23, 59, 155, DateTimeKind.Utc).AddTicks(9705));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 20, 18, 36, 40, 681, DateTimeKind.Utc).AddTicks(6806),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2025, 12, 20, 19, 23, 59, 155, DateTimeKind.Utc).AddTicks(9367));
        }
    }
}
