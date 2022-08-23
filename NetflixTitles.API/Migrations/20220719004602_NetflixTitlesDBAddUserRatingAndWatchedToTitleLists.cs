using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetflixTitles.API.Migrations
{
    public partial class NetflixTitlesDBAddUserRatingAndWatchedToTitleLists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserRating",
                table: "title_lists",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Watched",
                table: "title_lists",
                type: "tinyint(1)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserRating",
                table: "title_lists");

            migrationBuilder.DropColumn(
                name: "Watched",
                table: "title_lists");
        }
    }
}
