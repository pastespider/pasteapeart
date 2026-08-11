using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExampleApeart.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class database_init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PTGradeInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 4, nullable: true),
                    Desc = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Mark = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    IsEnable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTGradeInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PTGradeRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GradeId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTGradeRole", x => x.Id);
                },
                comment: "角色绑定权限");

            migrationBuilder.CreateTable(
                name: "PTRoleInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Model = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    Role = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Path = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Desc = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    RoleType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsEnable = table.Column<bool>(type: "INTEGER", nullable: false),
                    Sort = table.Column<int>(type: "INTEGER", nullable: false),
                    FatherId = table.Column<int>(type: "INTEGER", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    SortStr = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    RootId = table.Column<int>(type: "INTEGER", nullable: false),
                    FatherStr = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTRoleInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PTUserGrade",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    GradeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTUserGrade", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PTUserInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    UserHead = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Desc = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    PassWord = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    IsEnable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTUserInfo", x => x.Id);
                },
                comment: "登录用户表");

            migrationBuilder.CreateIndex(
                name: "IX_PTGradeInfo_Name",
                table: "PTGradeInfo",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PTUserInfo_Email",
                table: "PTUserInfo",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PTGradeInfo");

            migrationBuilder.DropTable(
                name: "PTGradeRole");

            migrationBuilder.DropTable(
                name: "PTRoleInfo");

            migrationBuilder.DropTable(
                name: "PTUserGrade");

            migrationBuilder.DropTable(
                name: "PTUserInfo");
        }
    }
}
