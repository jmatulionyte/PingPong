using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PingPongAPI.Migrations
{
    /// <inheritdoc />
    public partial class playersTblUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlayoffMatchNr",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Winner",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayoffMatchNr",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Winner",
                table: "Matches");
        }
    }
}
