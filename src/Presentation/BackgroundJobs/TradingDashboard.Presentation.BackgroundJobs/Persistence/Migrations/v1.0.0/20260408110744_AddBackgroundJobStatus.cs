using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingDashboard.Presentation.BackgroundJobs.Persistence.Migrations.v1._0._0
{
    /// <inheritdoc />
    public partial class AddBackgroundJobStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "backgroundjobs",
                table: "BackgroundJobs",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "Running");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "backgroundjobs",
                table: "BackgroundJobs");
        }
    }
}
