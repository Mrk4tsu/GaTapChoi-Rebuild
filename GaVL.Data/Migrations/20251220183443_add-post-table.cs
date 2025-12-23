using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaVL.Data.Migrations
{
    /// <inheritdoc />
    public partial class addposttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("9377d8eb-0056-481e-ac76-87d24ea3a976"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("436aa129-a8d2-4bc8-974c-a9fb96352df1"));

            migrationBuilder.CreateTable(
                name: "posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    main_image = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    seo_alias = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2025, 12, 20, 18, 34, 42, 922, DateTimeKind.Utc).AddTicks(800)),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2025, 12, 20, 18, 34, 42, 922, DateTimeKind.Utc).AddTicks(1291)),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_posts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_posts_is_deleted",
                table: "posts",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_posts_user_id",
                table: "posts",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "posts");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("436aa129-a8d2-4bc8-974c-a9fb96352df1"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("9377d8eb-0056-481e-ac76-87d24ea3a976"));
        }
    }
}
