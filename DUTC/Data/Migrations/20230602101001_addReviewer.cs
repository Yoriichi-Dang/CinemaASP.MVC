using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DUTC.Data.Migrations
{
    /// <inheritdoc />
    public partial class addReviewer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Reviewer",
                table: "Revenues",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reviewer",
                table: "Revenues");
        }
    }
}
