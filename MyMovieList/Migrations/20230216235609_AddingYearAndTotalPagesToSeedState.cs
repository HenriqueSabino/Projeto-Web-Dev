using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMovieList.Migrations
{
    /// <inheritdoc />
    public partial class AddingYearAndTotalPagesToSeedState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalPages",
                table: "SeedStates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "SeedStates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPages",
                table: "SeedStates");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "SeedStates");
        }
    }
}
