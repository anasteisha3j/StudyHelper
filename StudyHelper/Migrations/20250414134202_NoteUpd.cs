using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studyhelper.Migrations
{
    /// <inheritdoc />
    public partial class NoteUpd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Notes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
