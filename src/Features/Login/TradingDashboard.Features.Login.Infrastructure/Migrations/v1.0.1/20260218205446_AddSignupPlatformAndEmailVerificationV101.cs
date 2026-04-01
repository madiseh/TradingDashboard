using Microsoft.EntityFrameworkCore.Migrations;
using TradingDashboard.Infrastructure.Persistence.Common.Migrations;

#nullable disable

namespace TradingDashboard.Features.Login.Infrastructure.Migrations.v1._0._1
{
    /// <inheritdoc />
    public partial class AddSignupPlatformAndEmailVerificationV101 : BaseSchemaMigration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                schema: "auth",
                table: "AppUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SignupAppPlatform",
                schema: "auth",
                table: "AppUsers",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                schema: "auth",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "SignupAppPlatform",
                schema: "auth",
                table: "AppUsers");
        }
    }
}


