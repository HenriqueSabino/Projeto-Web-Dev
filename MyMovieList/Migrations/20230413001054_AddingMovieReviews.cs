using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMovieList.Migrations
{
    /// <inheritdoc />
    public partial class AddingMovieReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WatchListItem_AspNetUsers_UserId",
                table: "WatchListItem");

            migrationBuilder.DropForeignKey(
                name: "FK_WatchListItem_Movies_MovieId",
                table: "WatchListItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WatchListItem",
                table: "WatchListItem");

            migrationBuilder.RenameTable(
                name: "WatchListItem",
                newName: "WatchListItems");

            migrationBuilder.RenameIndex(
                name: "IX_WatchListItem_MovieId",
                table: "WatchListItems",
                newName: "IX_WatchListItems_MovieId");

            migrationBuilder.AlterColumn<Guid>(
                name: "MovieId",
                table: "WatchListItems",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WatchListItems",
                table: "WatchListItems",
                columns: new[] { "UserId", "MovieId" });

            migrationBuilder.CreateTable(
                name: "MovieReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Vote = table.Column<int>(type: "int", nullable: false),
                    Review = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieReviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MovieReviews_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieReviews_MovieId",
                table: "MovieReviews",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieReviews_UserId",
                table: "MovieReviews",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WatchListItems_AspNetUsers_UserId",
                table: "WatchListItems",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WatchListItems_Movies_MovieId",
                table: "WatchListItems",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WatchListItems_AspNetUsers_UserId",
                table: "WatchListItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WatchListItems_Movies_MovieId",
                table: "WatchListItems");

            migrationBuilder.DropTable(
                name: "MovieReviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WatchListItems",
                table: "WatchListItems");

            migrationBuilder.RenameTable(
                name: "WatchListItems",
                newName: "WatchListItem");

            migrationBuilder.RenameIndex(
                name: "IX_WatchListItems_MovieId",
                table: "WatchListItem",
                newName: "IX_WatchListItem_MovieId");

            migrationBuilder.AlterColumn<Guid>(
                name: "MovieId",
                table: "WatchListItem",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WatchListItem",
                table: "WatchListItem",
                columns: new[] { "UserId", "MovieId" });

            migrationBuilder.AddForeignKey(
                name: "FK_WatchListItem_AspNetUsers_UserId",
                table: "WatchListItem",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WatchListItem_Movies_MovieId",
                table: "WatchListItem",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
