using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceToCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                table: "CartItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ServiceId",
                table: "CartItems",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Services_ServiceId",
                table: "CartItems",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Services_ServiceId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ServiceId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "CartItems");
        }
    }
}
