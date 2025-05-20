using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AgroMonitor.Migrations
{
    /// <inheritdoc />
    public partial class CreateCustomerPackageJoinTableWithId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Customers_CustomerId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_CustomerId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Devices");

            migrationBuilder.CreateTable(
                name: "CustomerPackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    PackageId = table.Column<int>(type: "integer", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerPackages_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerPackages_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPackages_CustomerId",
                table: "CustomerPackages",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPackages_Id",
                table: "CustomerPackages",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPackages_PackageId",
                table: "CustomerPackages",
                column: "PackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerPackages");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Devices",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_CustomerId",
                table: "Devices",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Customers_CustomerId",
                table: "Devices",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");
        }
    }
}
