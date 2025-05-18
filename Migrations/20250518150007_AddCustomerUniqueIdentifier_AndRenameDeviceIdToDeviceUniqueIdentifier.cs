using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerUniqueIdentifier_AndRenameDeviceIdToDeviceUniqueIdentifier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeviceIdentifier",
                table: "Devices",
                newName: "DeviceUniqueIdentifier");

            migrationBuilder.AddColumn<string>(
                name: "CustomerUniqueIdentifier",
                table: "Customers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerUniqueIdentifier",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "DeviceUniqueIdentifier",
                table: "Devices",
                newName: "DeviceIdentifier");
        }
    }
}
