using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addedColumnsIntoCollesionAllerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DebrisAltitudeKm",
                table: "CollisionAlerts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DebrisLatitude",
                table: "CollisionAlerts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DebrisLongitude",
                table: "CollisionAlerts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SatAltitudeKm",
                table: "CollisionAlerts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SatLatitude",
                table: "CollisionAlerts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SatLongitude",
                table: "CollisionAlerts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DebrisAltitudeKm",
                table: "CollisionAlerts");

            migrationBuilder.DropColumn(
                name: "DebrisLatitude",
                table: "CollisionAlerts");

            migrationBuilder.DropColumn(
                name: "DebrisLongitude",
                table: "CollisionAlerts");

            migrationBuilder.DropColumn(
                name: "SatAltitudeKm",
                table: "CollisionAlerts");

            migrationBuilder.DropColumn(
                name: "SatLatitude",
                table: "CollisionAlerts");

            migrationBuilder.DropColumn(
                name: "SatLongitude",
                table: "CollisionAlerts");
        }
    }
}
