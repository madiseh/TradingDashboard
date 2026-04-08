using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingDashboard.Features.Login.Infrastructure.Migrations.v1._0._2
{
    /// <inheritdoc />
    public partial class AddUserRoleAndStatusAdminUsersV102 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                schema: "auth",
                table: "AppUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "auth",
                table: "AppUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                schema: "auth",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "auth",
                table: "AppUsers");
        }
    }
}
