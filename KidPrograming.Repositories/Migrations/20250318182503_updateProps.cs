using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KidPrograming.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class updateProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreativeAnswer",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreativeAnswer",
                table: "Submissions");
        }
    }
}
