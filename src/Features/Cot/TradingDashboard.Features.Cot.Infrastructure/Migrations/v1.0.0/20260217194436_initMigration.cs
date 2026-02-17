using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingDashboard.Features.Cot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cot");

            migrationBuilder.CreateTable(
                name: "CotInstrumentTypes",
                schema: "cot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false, collation: "NOCASE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotInstrumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CotInstruments",
                schema: "cot",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    InstrumentId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    InstrumentSlug = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false, collation: "NOCASE"),
                    InstrumentName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotInstruments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CotInstruments_CotInstrumentTypes_Type",
                        column: x => x.Type,
                        principalSchema: "cot",
                        principalTable: "CotInstrumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CotInstruments_InstrumentId",
                schema: "cot",
                table: "CotInstruments",
                column: "InstrumentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CotInstruments_InstrumentSlug",
                schema: "cot",
                table: "CotInstruments",
                column: "InstrumentSlug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CotInstruments_Type",
                schema: "cot",
                table: "CotInstruments",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_CotInstrumentTypes_Name",
                schema: "cot",
                table: "CotInstrumentTypes",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CotInstruments",
                schema: "cot");

            migrationBuilder.DropTable(
                name: "CotInstrumentTypes",
                schema: "cot");
        }
    }
}
