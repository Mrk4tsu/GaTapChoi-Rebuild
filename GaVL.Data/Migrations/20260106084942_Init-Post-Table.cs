using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GaVL.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitPostTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 1, 6, 8, 49, 42, 307, DateTimeKind.Utc).AddTicks(7902),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 1, 1, 12, 37, 4, 352, DateTimeKind.Utc).AddTicks(8125));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 1, 6, 8, 49, 42, 307, DateTimeKind.Utc).AddTicks(7364),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 1, 1, 12, 37, 4, 352, DateTimeKind.Utc).AddTicks(7562));

            migrationBuilder.AddColumn<int>(
                name: "category_id",
                table: "posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "sumary",
                table: "posts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "post_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    seo_alias = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    create_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "post_revisions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    post_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content_snapshot = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_revisions", x => x.id);
                    table.ForeignKey(
                        name: "FK_post_revisions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    seo_alias = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "post_tags",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "integer", nullable: false),
                    tag_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_tags", x => new { x.post_id, x.tag_id });
                    table.ForeignKey(
                        name: "FK_post_tags_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_post_tags_tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_posts_category_id",
                table: "posts",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_categories_id",
                table: "post_categories",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_post_categories_is_deleted",
                table: "post_categories",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_post_categories_seo_alias",
                table: "post_categories",
                column: "seo_alias");

            migrationBuilder.CreateIndex(
                name: "IX_post_revisions_id",
                table: "post_revisions",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_post_revisions_post_id",
                table: "post_revisions",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_revisions_user_id",
                table: "post_revisions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_tags_post_id",
                table: "post_tags",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_tags_tag_id",
                table: "post_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_tags_seo_alias",
                table: "tags",
                column: "seo_alias",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_post_categories_category_id",
                table: "posts",
                column: "category_id",
                principalTable: "post_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_posts_post_categories_category_id",
                table: "posts");

            migrationBuilder.DropTable(
                name: "post_categories");

            migrationBuilder.DropTable(
                name: "post_revisions");

            migrationBuilder.DropTable(
                name: "post_tags");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropIndex(
                name: "IX_posts_category_id",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "sumary",
                table: "posts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "update_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 1, 1, 12, 37, 4, 352, DateTimeKind.Utc).AddTicks(8125),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 1, 6, 8, 49, 42, 307, DateTimeKind.Utc).AddTicks(7902));

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 1, 1, 12, 37, 4, 352, DateTimeKind.Utc).AddTicks(7562),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 1, 6, 8, 49, 42, 307, DateTimeKind.Utc).AddTicks(7364));
        }
    }
}
