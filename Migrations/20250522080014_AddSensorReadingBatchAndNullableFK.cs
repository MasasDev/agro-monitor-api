using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AgroMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddSensorReadingBatchAndNullableFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BatchId",
                table: "SensorReadings",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReadingBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AISuggestion = table.Column<string>(type: "text", nullable: true),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReadingBatches_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_BatchId",
                table: "SensorReadings",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadingBatches_DeviceId",
                table: "ReadingBatches",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_SensorReadings_ReadingBatches_BatchId",
                table: "SensorReadings",
                column: "BatchId",
                principalTable: "ReadingBatches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorReadings_ReadingBatches_BatchId",
                table: "SensorReadings");

            migrationBuilder.DropTable(
                name: "ReadingBatches");

            migrationBuilder.DropIndex(
                name: "IX_SensorReadings_BatchId",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "SensorReadings");
        }
    }
}
