using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KidPrograming.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class updateSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Submissions_ChapterProgressId",
                table: "Submissions");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ChapterProgressId",
                table: "Submissions",
                column: "ChapterProgressId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Submissions_ChapterProgressId",
                table: "Submissions");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ChapterProgressId",
                table: "Submissions",
                column: "ChapterProgressId");
        }
    }
}
