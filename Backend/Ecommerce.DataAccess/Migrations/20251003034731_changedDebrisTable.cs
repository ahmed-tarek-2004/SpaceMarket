using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class changedDebrisTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Altitude",
                table: "Debris");

            migrationBuilder.DropColumn(
                name: "Velocity",
                table: "Debris");

            migrationBuilder.AlterColumn<string>(
                name: "NoradId",
                table: "Debris",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "TleLine1",
                table: "Debris",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TleLine2",
                table: "Debris",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Debris_NoradId",
                table: "Debris",
                column: "NoradId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Debris_NoradId",
                table: "Debris");

            migrationBuilder.DropColumn(
                name: "TleLine1",
                table: "Debris");

            migrationBuilder.DropColumn(
                name: "TleLine2",
                table: "Debris");

            migrationBuilder.AlterColumn<string>(
                name: "NoradId",
                table: "Debris",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<double>(
                name: "Altitude",
                table: "Debris",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Velocity",
                table: "Debris",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
