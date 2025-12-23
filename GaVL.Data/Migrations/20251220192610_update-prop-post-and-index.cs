using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaVL.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateproppostandindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("17a64a25-9b55-4e1a-8ef7-69d0f56f2af2"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("5ec9d72e-db34-483e-8ca3-ef743af736ff"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 20, 19, 26, 10, 364, DateTimeKind.Utc).AddTicks(9008),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2025, 12, 20, 19, 23, 59, 155, DateTimeKind.Utc).AddTicks(9705));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 20, 19, 26, 10, 364, DateTimeKind.Utc).AddTicks(8686),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2025, 12, 20, 19, 23, 59, 155, DateTimeKind.Utc).AddTicks(9367));

            migrationBuilder.CreateIndex(
                name: "IX_posts_code",
                table: "posts",
                column: "code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_posts_code",
                table: "posts");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("5ec9d72e-db34-483e-8ca3-ef743af736ff"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("17a64a25-9b55-4e1a-8ef7-69d0f56f2af2"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 20, 19, 23, 59, 155, DateTimeKind.Utc).AddTicks(9705),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2025, 12, 20, 19, 26, 10, 364, DateTimeKind.Utc).AddTicks(9008));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 20, 19, 23, 59, 155, DateTimeKind.Utc).AddTicks(9367),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2025, 12, 20, 19, 26, 10, 364, DateTimeKind.Utc).AddTicks(8686));
        }
    }
}
