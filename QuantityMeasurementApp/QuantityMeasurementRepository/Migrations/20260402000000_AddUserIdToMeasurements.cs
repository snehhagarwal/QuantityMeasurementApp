using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuantityMeasurementRepository.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToMeasurements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "user_id",
                table: "quantity_measurements",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_user_id",
                table: "quantity_measurements",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_user_id",
                table: "quantity_measurements");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "quantity_measurements");
        }
    }
}
