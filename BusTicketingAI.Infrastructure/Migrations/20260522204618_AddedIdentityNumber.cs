using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusTicketingAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedIdentityNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityNumber",
                table: "Users",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityNumber",
                table: "Users");
        }
    }
}
