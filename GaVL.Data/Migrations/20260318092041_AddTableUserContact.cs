using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaVL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTableUserContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 20, 38, 750, DateTimeKind.Utc).AddTicks(3480),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 17, 1, 58, 28, 712, DateTimeKind.Utc).AddTicks(1685));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 20, 38, 750, DateTimeKind.Utc).AddTicks(3335),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 17, 1, 58, 28, 712, DateTimeKind.Utc).AddTicks(1318));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 20, 38, 749, DateTimeKind.Utc).AddTicks(7297),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 17, 1, 58, 28, 710, DateTimeKind.Utc).AddTicks(8663));

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "UserContacts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    display_label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_public = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    position = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2026, 3, 18, 9, 20, 38, 752, DateTimeKind.Utc).AddTicks(7201)),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserContacts", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserContacts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orders_user_id",
                table: "orders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserContacts_user_id",
                table: "UserContacts",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_users_user_id",
                table: "orders",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_users_user_id",
                table: "orders");

            migrationBuilder.DropTable(
                name: "UserContacts");

            migrationBuilder.DropIndex(
                name: "IX_orders_user_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "orders");

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 17, 1, 58, 28, 712, DateTimeKind.Utc).AddTicks(1685),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 20, 38, 750, DateTimeKind.Utc).AddTicks(3480));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 17, 1, 58, 28, 712, DateTimeKind.Utc).AddTicks(1318),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 20, 38, 750, DateTimeKind.Utc).AddTicks(3335));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 17, 1, 58, 28, 710, DateTimeKind.Utc).AddTicks(8663),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 20, 38, 749, DateTimeKind.Utc).AddTicks(7297));
        }
    }
}
