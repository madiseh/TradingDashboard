using Microsoft.EntityFrameworkCore.Migrations;
using TradingDashboard.Infrastructure.Persistence.Common.Migrations;

#nullable disable

namespace TradingDashboard.Features.UserManagement.Infrastructure.Migrations.v1._0._3
{
    /// <inheritdoc />
    public partial class AddAppUserStatusV103 : BaseSchemaMigration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "auth",
                table: "AppUsers",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "Active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "auth",
                table: "AppUsers");
        }
    }
}
