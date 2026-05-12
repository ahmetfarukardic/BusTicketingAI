using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusTicketingAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTripStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 1);
            migrationBuilder.Sql("UPDATE Trips SET Status = 2 WHERE DepartureTime <= GETDATE()");
            migrationBuilder.Sql("UPDATE Trips SET Status = 1 WHERE DepartureTime > GETDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Trips");
        }
    }
}
