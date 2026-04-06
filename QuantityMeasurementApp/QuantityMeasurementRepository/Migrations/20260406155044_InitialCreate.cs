using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuantityMeasurementRepository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "quantity_measurements",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    operation_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    first_operand_value = table.Column<double>(type: "float", nullable: true),
                    first_operand_unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    first_operand_category = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    first_operand_display = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    second_operand_value = table.Column<double>(type: "float", nullable: true),
                    second_operand_unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    second_operand_category = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    second_operand_display = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    target_unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    result_value = table.Column<double>(type: "float", nullable: true),
                    result_unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    result_measurement_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    formatted_result = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    is_successful = table.Column<bool>(type: "bit", nullable: false),
                    error_details = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quantity_measurements", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    google_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    picture_url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_created_at",
                table: "quantity_measurements",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_is_successful",
                table: "quantity_measurements",
                column: "is_successful");

            migrationBuilder.CreateIndex(
                name: "idx_measurement_type",
                table: "quantity_measurements",
                column: "first_operand_category");

            migrationBuilder.CreateIndex(
                name: "idx_operation_type",
                table: "quantity_measurements",
                column: "operation_type");

            migrationBuilder.CreateIndex(
                name: "idx_user_id",
                table: "quantity_measurements",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quantity_measurements");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
