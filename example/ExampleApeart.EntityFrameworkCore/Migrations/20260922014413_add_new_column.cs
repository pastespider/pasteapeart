using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExampleApeart.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class add_new_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "PTUserInfo",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "UserHead",
                table: "PTUserInfo",
                newName: "Head");

            migrationBuilder.RenameColumn(
                name: "RootId",
                table: "PTRoleInfo",
                newName: "IsDefault");

            migrationBuilder.AlterTable(
                name: "PTGradeRole",
                comment: "角色权限",
                oldComment: "角色绑定权限");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "PTUserInfo",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "Head",
                table: "PTUserInfo",
                newName: "UserHead");

            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "PTRoleInfo",
                newName: "RootId");

            migrationBuilder.AlterTable(
                name: "PTGradeRole",
                comment: "角色绑定权限",
                oldComment: "角色权限");
        }
    }
}
