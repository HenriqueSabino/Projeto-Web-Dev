using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMovieList.Migrations
{
    /// <inheritdoc />
    public partial class AddingMovieSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Movies");
        }
    }
}
