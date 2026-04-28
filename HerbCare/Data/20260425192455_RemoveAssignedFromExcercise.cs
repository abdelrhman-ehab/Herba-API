using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HerbCare.Data
{
    /// <inheritdoc />
    public partial class RemoveAssignedFromExcercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignmentExercise",
                table: "Exercises");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignmentExercise",
                table: "Exercises",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }
    }
}
