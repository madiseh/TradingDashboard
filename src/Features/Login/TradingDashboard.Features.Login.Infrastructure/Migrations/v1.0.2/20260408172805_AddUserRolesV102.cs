using Microsoft.EntityFrameworkCore.Migrations;
using TradingDashboard.Infrastructure.Persistence.Common.Migrations;

#nullable disable

namespace TradingDashboard.Features.Login.Infrastructure.Migrations.v1._0._2
{
    /// <inheritdoc />
    public partial class AddUserRolesV102 : BaseSchemaMigration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                schema: "auth",
                table: "AppUsers",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "Guest");

            migrationBuilder.Sql("""
                UPDATE [auth].[AppUsers]
                SET [Role] = 'SuperAdmin'
                WHERE [Username] = 'admin' OR [Email] = 'admin@tradingdashboard.com';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                schema: "auth",
                table: "AppUsers");
        }
    }
}
