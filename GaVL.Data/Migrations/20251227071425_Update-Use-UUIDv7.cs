using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaVL.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUseUUIDv7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "users",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("73f232b7-75ce-4620-acc0-3b98956fd14f"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 27, 7, 14, 24, 889, DateTimeKind.Utc).AddTicks(554),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 12, 24, 5, 51, 9, 246, DateTimeKind.Utc).AddTicks(2538));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 27, 7, 14, 24, 889, DateTimeKind.Utc).AddTicks(287),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 12, 24, 5, 51, 9, 246, DateTimeKind.Utc).AddTicks(2283));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("73f232b7-75ce-4620-acc0-3b98956fd14f"),
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 24, 5, 51, 9, 246, DateTimeKind.Utc).AddTicks(2538),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 12, 27, 7, 14, 24, 889, DateTimeKind.Utc).AddTicks(554));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 12, 24, 5, 51, 9, 246, DateTimeKind.Utc).AddTicks(2283),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 12, 27, 7, 14, 24, 889, DateTimeKind.Utc).AddTicks(287));
        }
    }
}
