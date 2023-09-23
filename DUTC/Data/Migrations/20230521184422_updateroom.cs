using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DUTC.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateroom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoomID",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomID",
                table: "Rooms");
        }
    }
}
