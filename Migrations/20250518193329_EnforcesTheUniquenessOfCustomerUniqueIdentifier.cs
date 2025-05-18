using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroMonitor.Migrations
{
    /// <inheritdoc />
    public partial class EnforcesTheUniquenessOfCustomerUniqueIdentifier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerUniqueIdentifier",
                table: "Customers",
                column: "CustomerUniqueIdentifier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_CustomerUniqueIdentifier",
                table: "Customers");
        }
    }
}
