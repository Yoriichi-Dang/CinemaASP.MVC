using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DUTC.Data.Migrations
{
    /// <inheritdoc />
    public partial class newShowtimeRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomID",
                table: "Showtimes");

            migrationBuilder.DropColumn(
                name: "RoomID",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "xPosition",
                table: "Seats");

            migrationBuilder.RenameColumn(
                name: "ShowtimeID",
                table: "Showtimes",
                newName: "MovieID");

            migrationBuilder.RenameColumn(
                name: "yPosition",
                table: "Seats",
                newName: "ShowtimeRoomID");

            migrationBuilder.CreateTable(
                name: "ShowtimeRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShowtimeId = table.Column<int>(type: "int", nullable: false),
                    idRoom = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowtimeRooms", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShowtimeRooms");

            migrationBuilder.RenameColumn(
                name: "MovieID",
                table: "Showtimes",
                newName: "ShowtimeID");

            migrationBuilder.RenameColumn(
                name: "ShowtimeRoomID",
                table: "Seats",
                newName: "yPosition");

            migrationBuilder.AddColumn<int>(
                name: "RoomID",
                table: "Showtimes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoomID",
                table: "Seats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "xPosition",
                table: "Seats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
