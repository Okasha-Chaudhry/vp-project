using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyConnect.API.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoCallFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UseBuiltInVideoCall",
                table: "StudySessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseBuiltInVideoCall",
                table: "StudySessions");
        }
    }
}
