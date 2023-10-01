using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeReport.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeDailyHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "DailyHours",
                schema: "time_report",
                table: "employee",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyHours",
                schema: "time_report",
                table: "employee");
        }
    }
}
