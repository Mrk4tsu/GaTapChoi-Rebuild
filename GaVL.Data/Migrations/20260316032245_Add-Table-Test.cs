using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GaVL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTableTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 16, 3, 22, 45, 260, DateTimeKind.Utc).AddTicks(5912),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 12, 7, 46, 36, 933, DateTimeKind.Utc).AddTicks(2691));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 16, 3, 22, 45, 260, DateTimeKind.Utc).AddTicks(5614),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 12, 7, 46, 36, 933, DateTimeKind.Utc).AddTicks(2391));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 16, 3, 22, 45, 259, DateTimeKind.Utc).AddTicks(3071),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 12, 7, 46, 36, 931, DateTimeKind.Utc).AddTicks(1941));

            migrationBuilder.CreateTable(
                name: "testdb",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testdb", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "testdb");

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 12, 7, 46, 36, 933, DateTimeKind.Utc).AddTicks(2691),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 16, 3, 22, 45, 260, DateTimeKind.Utc).AddTicks(5912));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 12, 7, 46, 36, 933, DateTimeKind.Utc).AddTicks(2391),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 16, 3, 22, 45, 260, DateTimeKind.Utc).AddTicks(5614));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 12, 7, 46, 36, 931, DateTimeKind.Utc).AddTicks(1941),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 16, 3, 22, 45, 259, DateTimeKind.Utc).AddTicks(3071));
        }
    }
}
