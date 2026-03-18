using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaVL.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserCOntact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserContacts_users_user_id",
                table: "UserContacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserContacts",
                table: "UserContacts");

            migrationBuilder.RenameTable(
                name: "UserContacts",
                newName: "user_contacts");

            migrationBuilder.RenameIndex(
                name: "IX_UserContacts_user_id",
                table: "user_contacts",
                newName: "IX_user_contacts_user_id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 39, 26, 4, DateTimeKind.Utc).AddTicks(2846),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 36, 18, 920, DateTimeKind.Utc).AddTicks(8453));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 39, 26, 4, DateTimeKind.Utc).AddTicks(2699),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 36, 18, 920, DateTimeKind.Utc).AddTicks(8302));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 39, 26, 3, DateTimeKind.Utc).AddTicks(6928),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 36, 18, 920, DateTimeKind.Utc).AddTicks(1713));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user_contacts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 39, 26, 6, DateTimeKind.Utc).AddTicks(8427),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 36, 18, 923, DateTimeKind.Utc).AddTicks(6958));

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_contacts",
                table: "user_contacts",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_contacts_users_user_id",
                table: "user_contacts",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_contacts_users_user_id",
                table: "user_contacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_contacts",
                table: "user_contacts");

            migrationBuilder.RenameTable(
                name: "user_contacts",
                newName: "UserContacts");

            migrationBuilder.RenameIndex(
                name: "IX_user_contacts_user_id",
                table: "UserContacts",
                newName: "IX_UserContacts_user_id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 36, 18, 920, DateTimeKind.Utc).AddTicks(8453),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 39, 26, 4, DateTimeKind.Utc).AddTicks(2846));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 36, 18, 920, DateTimeKind.Utc).AddTicks(8302),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 39, 26, 4, DateTimeKind.Utc).AddTicks(2699));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 36, 18, 920, DateTimeKind.Utc).AddTicks(1713),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 39, 26, 3, DateTimeKind.Utc).AddTicks(6928));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "UserContacts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 18, 9, 36, 18, 923, DateTimeKind.Utc).AddTicks(6958),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 18, 9, 39, 26, 6, DateTimeKind.Utc).AddTicks(8427));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserContacts",
                table: "UserContacts",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserContacts_users_user_id",
                table: "UserContacts",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
