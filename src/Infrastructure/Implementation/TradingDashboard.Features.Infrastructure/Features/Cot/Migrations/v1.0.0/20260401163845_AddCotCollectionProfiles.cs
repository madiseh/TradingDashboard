using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingDashboard.Features.Cot.Infrastructure.Migrations.v1._0._0
{
    /// <inheritdoc />
    public partial class AddCotCollectionProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CotCollectionJobStates",
                schema: "cot",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LastStartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastCompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSucceededAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSavedFileCount = table.Column<int>(type: "int", nullable: false),
                    LastFetchedProfileCount = table.Column<int>(type: "int", nullable: false),
                    LastError = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotCollectionJobStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CotInstrumentCollectionProfiles",
                schema: "cot",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NormalMarketAndExchangeName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    LegacyMarketAndExchangeName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsNormalEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsLegacyEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotInstrumentCollectionProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CotInstrumentCollectionProfiles_CotInstruments_Id",
                        column: x => x.Id,
                        principalSchema: "cot",
                        principalTable: "CotInstruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CotCollectionJobStates",
                schema: "cot");

            migrationBuilder.DropTable(
                name: "CotInstrumentCollectionProfiles",
                schema: "cot");
        }
    }
}
