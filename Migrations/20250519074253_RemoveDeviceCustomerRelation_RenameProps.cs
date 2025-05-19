using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroMonitor.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDeviceCustomerRelation_RenameProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Customers_CustomerId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "IsEquipmentReturned",
                table: "Customers",
                newName: "AreRentedDevicesReturned");

            migrationBuilder.RenameColumn(
                name: "EquipmentReturnDate",
                table: "Customers",
                newName: "RentedDevicesReturnDate");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Devices",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Customers_CustomerId",
                table: "Devices",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Customers_CustomerId",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "RentedDevicesReturnDate",
                table: "Customers",
                newName: "EquipmentReturnDate");

            migrationBuilder.RenameColumn(
                name: "AreRentedDevicesReturned",
                table: "Customers",
                newName: "IsEquipmentReturned");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Devices",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Devices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Customers_CustomerId",
                table: "Devices",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
