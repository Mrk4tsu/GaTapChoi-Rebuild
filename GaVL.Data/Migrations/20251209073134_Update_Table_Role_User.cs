using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaVL.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Table_Role_User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_roles_RoleId",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "users",
                newName: "role_id");

            migrationBuilder.RenameIndex(
                name: "IX_users_RoleId",
                table: "users",
                newName: "IX_users_role_id");

            migrationBuilder.AlterColumn<int>(
                name: "role_id",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 4,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_role_id",
                table: "users",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_roles_role_id",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "users",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_users_role_id",
                table: "users",
                newName: "IX_users_RoleId");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 4);

            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_RoleId",
                table: "users",
                column: "RoleId",
                principalTable: "roles",
                principalColumn: "Id");
        }
    }
}
