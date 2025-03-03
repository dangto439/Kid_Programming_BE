using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KidPrograming.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class update_lab : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Labs_Lessons_LessionId",
                table: "Labs");

            migrationBuilder.DropForeignKey(
                name: "FK_Labs_Lessons_LessonId",
                table: "Labs");

            migrationBuilder.DropIndex(
                name: "IX_Labs_LessionId",
                table: "Labs");

            migrationBuilder.DropColumn(
                name: "LessionId",
                table: "Labs");

            migrationBuilder.AlterColumn<string>(
                name: "LessonId",
                table: "Labs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LessonId1",
                table: "Labs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Labs_LessonId1",
                table: "Labs",
                column: "LessonId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Labs_Lessons_LessonId",
                table: "Labs",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Labs_Lessons_LessonId1",
                table: "Labs",
                column: "LessonId1",
                principalTable: "Lessons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Labs_Lessons_LessonId",
                table: "Labs");

            migrationBuilder.DropForeignKey(
                name: "FK_Labs_Lessons_LessonId1",
                table: "Labs");

            migrationBuilder.DropIndex(
                name: "IX_Labs_LessonId1",
                table: "Labs");

            migrationBuilder.DropColumn(
                name: "LessonId1",
                table: "Labs");

            migrationBuilder.AlterColumn<string>(
                name: "LessonId",
                table: "Labs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "LessionId",
                table: "Labs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Labs_LessionId",
                table: "Labs",
                column: "LessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Labs_Lessons_LessionId",
                table: "Labs",
                column: "LessionId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Labs_Lessons_LessonId",
                table: "Labs",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id");
        }
    }
}
