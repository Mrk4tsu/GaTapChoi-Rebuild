using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaVL.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUnqueSeoAliasCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_post_categories_seo_alias",
                table: "post_categories");

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 1, 7, 7, 51, 8, 943, DateTimeKind.Utc).AddTicks(8222),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 1, 6, 8, 49, 42, 307, DateTimeKind.Utc).AddTicks(7902));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 1, 7, 7, 51, 8, 943, DateTimeKind.Utc).AddTicks(7935),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 1, 6, 8, 49, 42, 307, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.CreateIndex(
                name: "IX_post_categories_seo_alias",
                table: "post_categories",
                column: "seo_alias",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_post_categories_seo_alias",
                table: "post_categories");

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 1, 6, 8, 49, 42, 307, DateTimeKind.Utc).AddTicks(7902),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 1, 7, 7, 51, 8, 943, DateTimeKind.Utc).AddTicks(8222));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 1, 6, 8, 49, 42, 307, DateTimeKind.Utc).AddTicks(7364),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 1, 7, 7, 51, 8, 943, DateTimeKind.Utc).AddTicks(7935));

            migrationBuilder.CreateIndex(
                name: "IX_post_categories_seo_alias",
                table: "post_categories",
                column: "seo_alias");
        }
    }
}
