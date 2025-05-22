using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandCodeToDeviceForDisplay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BrandCode",
                table: "DevicesForDisplay",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrandCode",
                table: "DevicesForDisplay");
        }
    }
}
