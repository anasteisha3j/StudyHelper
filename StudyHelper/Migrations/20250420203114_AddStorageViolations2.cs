using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studyhelper.Migrations
{
    /// <inheritdoc />
    public partial class AddStorageViolations2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StorageViolations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    UserEmail = table.Column<string>(type: "TEXT", nullable: false),
                    ViolationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AttemptedFileType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AttemptedSize = table.Column<long>(type: "INTEGER", nullable: false),
                    MaxAllowed = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageViolations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StorageViolations");
        }
    }
}
