using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaVL.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "UserContacts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 22, 2, 218, DateTimeKind.Utc).AddTicks(2399),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 20, 38, 752, DateTimeKind.Utc).AddTicks(7201));

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 22, 2, 215, DateTimeKind.Utc).AddTicks(8043),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 20, 38, 750, DateTimeKind.Utc).AddTicks(3480));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 22, 2, 215, DateTimeKind.Utc).AddTicks(7900),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 20, 38, 750, DateTimeKind.Utc).AddTicks(3335));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 22, 2, 215, DateTimeKind.Utc).AddTicks(1316),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 20, 38, 749, DateTimeKind.Utc).AddTicks(7297));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "UserContacts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 20, 38, 752, DateTimeKind.Utc).AddTicks(7201),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 22, 2, 218, DateTimeKind.Utc).AddTicks(2399));

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 20, 38, 750, DateTimeKind.Utc).AddTicks(3480),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 22, 2, 215, DateTimeKind.Utc).AddTicks(8043));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 20, 38, 750, DateTimeKind.Utc).AddTicks(3335),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 22, 2, 215, DateTimeKind.Utc).AddTicks(7900));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 20, 38, 749, DateTimeKind.Utc).AddTicks(7297),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 22, 2, 215, DateTimeKind.Utc).AddTicks(1316));
        }
    }
}
