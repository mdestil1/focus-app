using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotifyTestApp.Migrations
{
    /// <inheritdoc />
    public partial class CreateStudySessionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "StudySessions",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudySessions",
                table: "StudySessions",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StudySessions",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "StudySessions");
        }
    }
}
