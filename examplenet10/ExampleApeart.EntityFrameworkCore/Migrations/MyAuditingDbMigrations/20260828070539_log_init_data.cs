using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExampleApeart.EntityFrameworkCore.Migrations.MyAuditingDbMigrations
{
    /// <inheritdoc />
    public partial class log_init_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApAuditLog",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LogId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    HttpMethod = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    HttpStatusCode = table.Column<int>(type: "INTEGER", nullable: false),
                    RequestBody = table.Column<string>(type: "TEXT", nullable: true),
                    QueryString = table.Column<string>(type: "TEXT", nullable: true),
                    Headers = table.Column<string>(type: "TEXT", nullable: true),
                    ResponseBody = table.Column<string>(type: "TEXT", nullable: true),
                    ExecutionDuration = table.Column<long>(type: "INTEGER", nullable: false),
                    Exception = table.Column<string>(type: "TEXT", nullable: true),
                    HasException = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasEntityChange = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExecutionTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApAuditLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApEntityChangeInfo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LogId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    EntityType = table.Column<string>(type: "TEXT", nullable: true),
                    EntityId = table.Column<string>(type: "TEXT", nullable: true),
                    ChangeType = table.Column<int>(type: "INTEGER", nullable: false),
                    ChangeTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ApAuditLogId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApEntityChangeInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApEntityChangeInfo_ApAuditLog_ApAuditLogId",
                        column: x => x.ApAuditLogId,
                        principalTable: "ApAuditLog",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApPropertyChangeInfo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LogId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    PropertyName = table.Column<string>(type: "TEXT", nullable: true),
                    OriginalValue = table.Column<string>(type: "TEXT", nullable: true),
                    NewValue = table.Column<string>(type: "TEXT", nullable: true),
                    ValueType = table.Column<string>(type: "TEXT", nullable: true),
                    ApEntityChangeInfoId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApPropertyChangeInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApPropertyChangeInfo_ApEntityChangeInfo_ApEntityChangeInfoId",
                        column: x => x.ApEntityChangeInfoId,
                        principalTable: "ApEntityChangeInfo",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApEntityChangeInfo_ApAuditLogId",
                table: "ApEntityChangeInfo",
                column: "ApAuditLogId");

            migrationBuilder.CreateIndex(
                name: "IX_ApPropertyChangeInfo_ApEntityChangeInfoId",
                table: "ApPropertyChangeInfo",
                column: "ApEntityChangeInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApPropertyChangeInfo");

            migrationBuilder.DropTable(
                name: "ApEntityChangeInfo");

            migrationBuilder.DropTable(
                name: "ApAuditLog");
        }
    }
}
