using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingDashboard.Presentation.BackgroundJobs.Persistence.Migrations.v1._0._0
{
    /// <inheritdoc />
    public partial class InitialBackgroundJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "backgroundjobs");

            migrationBuilder.CreateTable(
                name: "BackgroundJobExecutionLogs",
                schema: "backgroundjobs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    JobGroup = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TriggerName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TriggerGroup = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    ExceptionType = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ExecutedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundJobExecutionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BackgroundJobs",
                schema: "backgroundjobs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    JobName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    JobGroup = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TriggerName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TriggerGroup = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    JobType = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundJobs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundJobs_JobName_JobGroup",
                schema: "backgroundjobs",
                table: "BackgroundJobs",
                columns: new[] { "JobName", "JobGroup" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackgroundJobExecutionLogs",
                schema: "backgroundjobs");

            migrationBuilder.DropTable(
                name: "BackgroundJobs",
                schema: "backgroundjobs");
        }
    }
}
