using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExampleForm.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class init_database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PTArticleInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Sort = table.Column<int>(type: "INTEGER", nullable: false),
                    IsEnable = table.Column<bool>(type: "INTEGER", nullable: false),
                    Desc = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Body = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTArticleInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PTDynamicField",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TableId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    DataType = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    DefaultValue = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    Placeholder = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Maxlength = table.Column<int>(type: "INTEGER", nullable: false),
                    Required = table.Column<bool>(type: "INTEGER", nullable: false),
                    MatchRegex = table.Column<bool>(type: "INTEGER", nullable: false),
                    AttributesJson = table.Column<string>(type: "TEXT", nullable: true),
                    IsEnable = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReadOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowList = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowAdd = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowEdit = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowView = table.Column<bool>(type: "INTEGER", nullable: false),
                    NeedRemove = table.Column<bool>(type: "INTEGER", nullable: false),
                    Sort = table.Column<int>(type: "INTEGER", nullable: false),
                    ShowMark = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTDynamicField", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PTDynamicTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    KeyType = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    KeyWord = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Path = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    MatchRegex = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    Desc = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    IsEnable = table.Column<bool>(type: "INTEGER", nullable: false),
                    AttributeJson = table.Column<string>(type: "TEXT", nullable: true),
                    ReadOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanAdd = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanEdit = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanDel = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanView = table.Column<bool>(type: "INTEGER", nullable: false),
                    Sort = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTDynamicTable", x => x.Id);
                });

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
                name: "PTGuidTest1",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    Desc = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTGuidTest1", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PTIncludeTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    Desc = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTIncludeTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PTMyTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Age = table.Column<int>(type: "INTEGER", nullable: false),
                    Rate = table.Column<double>(type: "REAL", nullable: false),
                    IsEnable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTMyTable", x => x.Id);
                });

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
                name: "PTTestTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Head = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Age = table.Column<int>(type: "INTEGER", nullable: false),
                    DateStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Balance = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTTestTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PTUintTest1",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    Desc = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTUintTest1", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PTUploadImageInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Dir = table.Column<string>(type: "TEXT", maxLength: 8, nullable: true),
                    Path = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Url = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Md5 = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Size = table.Column<int>(type: "INTEGER", nullable: false),
                    Extension = table.Column<string>(type: "TEXT", maxLength: 8, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Width = table.Column<int>(type: "INTEGER", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: false),
                    UploadCloud = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTUploadImageInfo", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "PTParentTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Age = table.Column<int>(type: "INTEGER", nullable: false),
                    FrontId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsEnable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTParentTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PTParentTable_PTIncludeTable_FrontId",
                        column: x => x.FrontId,
                        principalTable: "PTIncludeTable",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PTTestLinkInclude",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MyTableId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    GradeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTTestLinkInclude", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PTTestLinkInclude_PTMyTable_MyTableId",
                        column: x => x.MyTableId,
                        principalTable: "PTMyTable",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PTDynamicTable_Code",
                table: "PTDynamicTable",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PTGradeInfo_Name",
                table: "PTGradeInfo",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PTParentTable_FrontId",
                table: "PTParentTable",
                column: "FrontId");

            migrationBuilder.CreateIndex(
                name: "IX_PTTestLinkInclude_MyTableId",
                table: "PTTestLinkInclude",
                column: "MyTableId");

            migrationBuilder.CreateIndex(
                name: "IX_PTTestTable_UserId",
                table: "PTTestTable",
                column: "UserId",
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
                name: "PTArticleInfo");

            migrationBuilder.DropTable(
                name: "PTDynamicField");

            migrationBuilder.DropTable(
                name: "PTDynamicTable");

            migrationBuilder.DropTable(
                name: "PTGradeInfo");

            migrationBuilder.DropTable(
                name: "PTGradeRole");

            migrationBuilder.DropTable(
                name: "PTGuidTest1");

            migrationBuilder.DropTable(
                name: "PTParentTable");

            migrationBuilder.DropTable(
                name: "PTRoleInfo");

            migrationBuilder.DropTable(
                name: "PTTestLinkInclude");

            migrationBuilder.DropTable(
                name: "PTTestTable");

            migrationBuilder.DropTable(
                name: "PTUintTest1");

            migrationBuilder.DropTable(
                name: "PTUploadImageInfo");

            migrationBuilder.DropTable(
                name: "PTUserGrade");

            migrationBuilder.DropTable(
                name: "PTUserInfo");

            migrationBuilder.DropTable(
                name: "PTIncludeTable");

            migrationBuilder.DropTable(
                name: "PTMyTable");
        }
    }
}
