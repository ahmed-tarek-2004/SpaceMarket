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
                defaultValue: Guid.Empty);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ServiceId",
                table: "OrderItems",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Services_ServiceId",
                table: "OrderItems",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Services_ServiceId",
                table: "OrderItems");

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
