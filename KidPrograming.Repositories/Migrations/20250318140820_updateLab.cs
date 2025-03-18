using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KidPrograming.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class updateLab : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Solution",
                table: "Labs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ToolBox",
                table: "Labs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Solution",
                table: "Labs");

            migrationBuilder.DropColumn(
                name: "ToolBox",
                table: "Labs");
        }
    }
}
