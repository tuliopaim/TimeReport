using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeReport.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeEntryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_company_CompanyId",
                schema: "time_report",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_user_CompanyId",
                schema: "time_report",
                table: "user");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "time_report",
                table: "user");

            migrationBuilder.CreateTable(
                name: "time-entry",
                schema: "time_report",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time-entry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_time-entry_employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "time_report",
                        principalTable: "employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_time-entry_EmployeeId",
                schema: "time_report",
                table: "time-entry",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_time-entry_Id_Time",
                schema: "time_report",
                table: "time-entry",
                columns: new[] { "Id", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_time-entry_Time",
                schema: "time_report",
                table: "time-entry",
                column: "Time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "time-entry",
                schema: "time_report");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "time_report",
                table: "user",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_CompanyId",
                schema: "time_report",
                table: "user",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_company_CompanyId",
                schema: "time_report",
                table: "user",
                column: "CompanyId",
                principalSchema: "time_report",
                principalTable: "company",
                principalColumn: "Id");
        }
    }
}
