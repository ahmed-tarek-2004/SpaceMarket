using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddingServieToOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagesUrl",
                table: "Services",
                newName: "ImagesUrl");

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                table: "OrderItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ServiceMetrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceMetrics_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ServiceId",
                table: "OrderItems",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceMetrics_ServiceId",
                table: "ServiceMetrics",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Services_ServiceId",
                table: "OrderItems",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Services_ServiceId",
                table: "OrderItems");

            migrationBuilder.DropTable(
                name: "ServiceMetrics");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ServiceId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "ImagesUrl",
                table: "Services",
                newName: "ImagesUrlJson");
        }
    }
}
