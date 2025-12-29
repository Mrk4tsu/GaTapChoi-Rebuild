using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaVL.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdMod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 29, 6, 1, 25, 165, DateTimeKind.Utc).AddTicks(8240),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 12, 29, 5, 50, 1, 162, DateTimeKind.Utc).AddTicks(2229));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 29, 6, 1, 25, 165, DateTimeKind.Utc).AddTicks(7964),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 12, 29, 5, 50, 1, 162, DateTimeKind.Utc).AddTicks(1965));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 29, 5, 50, 1, 162, DateTimeKind.Utc).AddTicks(2229),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 12, 29, 6, 1, 25, 165, DateTimeKind.Utc).AddTicks(8240));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 29, 5, 50, 1, 162, DateTimeKind.Utc).AddTicks(1965),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 12, 29, 6, 1, 25, 165, DateTimeKind.Utc).AddTicks(7964));
        }
    }
}
