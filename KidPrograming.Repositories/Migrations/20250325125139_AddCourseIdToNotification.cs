using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KidPrograming.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseIdToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourseId",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Notifications");
        }
    }
}
