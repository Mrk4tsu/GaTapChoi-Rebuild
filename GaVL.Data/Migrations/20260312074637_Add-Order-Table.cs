using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GaVL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 12, 7, 46, 36, 933, DateTimeKind.Utc).AddTicks(2691),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 6, 5, 12, 0, 929, DateTimeKind.Utc).AddTicks(8903));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 12, 7, 46, 36, 933, DateTimeKind.Utc).AddTicks(2391),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 6, 5, 12, 0, 929, DateTimeKind.Utc).AddTicks(8573));

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    number_phone = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    payment_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    create_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2026, 3, 12, 7, 46, 36, 931, DateTimeKind.Utc).AddTicks(1941))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payment_transactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sepay_id = table.Column<int>(type: "integer", nullable: true),
                    gateway = table.Column<string>(type: "text", nullable: true),
                    transaction_date = table.Column<string>(type: "text", nullable: true),
                    account_number = table.Column<string>(type: "text", nullable: true),
                    sub_account = table.Column<string>(type: "text", nullable: true),
                    amount_in = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    amount_out = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    accumulated = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    code = table.Column<string>(type: "text", nullable: true),
                    transaction_content = table.Column<string>(type: "text", nullable: true),
                    reference_number = table.Column<string>(type: "text", nullable: true),
                    body = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_transactions", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "payment_transactions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 6, 5, 12, 0, 929, DateTimeKind.Utc).AddTicks(8903),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 12, 7, 46, 36, 933, DateTimeKind.Utc).AddTicks(2691));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 6, 5, 12, 0, 929, DateTimeKind.Utc).AddTicks(8573),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 3, 12, 7, 46, 36, 933, DateTimeKind.Utc).AddTicks(2391));
        }
    }
}
