using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EditedCertificationsUrlsPropertyInServiceProviderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificationsUrlJson",
                table: "ServiceProviders");

            migrationBuilder.AddColumn<string>(
                name: "CertificationsUrls",
                table: "ServiceProviders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificationsUrls",
                table: "ServiceProviders");

            migrationBuilder.AddColumn<string>(
                name: "CertificationsUrlJson",
                table: "ServiceProviders",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }
    }
}
