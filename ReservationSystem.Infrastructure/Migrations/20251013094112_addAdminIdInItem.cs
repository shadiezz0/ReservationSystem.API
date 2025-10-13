using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addAdminIdInItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Items");
        }
    }
}
