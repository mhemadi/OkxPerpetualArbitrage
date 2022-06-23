using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OkxPerpetualArbitrage.Persistance.Migrations
{
    public partial class createdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FundingIncomes",
                columns: table => new
                {
                    FundingIncomeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BillId = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Symbol = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    Amount = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    IsInCurrentPosition = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundingIncomes", x => x.FundingIncomeId);
                });

            migrationBuilder.CreateTable(
                name: "PositionDemands",
                columns: table => new
                {
                    PositionDemandId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    PositionDemandState = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalSize = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    Filled = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    OpenDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    Spread = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    ActualSpread = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    PositionDemandSide = table.Column<int>(type: "INTEGER", nullable: false),
                    IsInstant = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsCanceled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionDemands", x => x.PositionDemandId);
                });

            migrationBuilder.CreateTable(
                name: "PositionHistory",
                columns: table => new
                {
                    PositionHistoryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    OpenDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    CloseDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    SpotTrade = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    PerpTrade = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    Fee = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    Funding = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    TotalPNL = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionHistory", x => x.PositionHistoryId);
                });

            migrationBuilder.CreateTable(
                name: "PotentialPositionRatingHistory",
                columns: table => new
                {
                    PotentialPositionRatingHistoryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Rating = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PotentialPositionRatingHistory", x => x.PotentialPositionRatingHistoryId);
                });

            migrationBuilder.CreateTable(
                name: "PotentialPositions",
                columns: table => new
                {
                    PotentialPositionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Funding = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    NextFunding = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    Spread = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    CloseSpread = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    Rating = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    MinSizeSpot = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    LotSizeSpot = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    TickSizeSpot = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    MinSizePerp = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    TickSizePerp = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    ContractValuePerp = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    FeeSpotMaker = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    FeeSpotTaker = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    FeePerpMaker = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    FeePerpTaker = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    MarkPrice = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    IsUpToDate = table.Column<bool>(type: "INTEGER", nullable: false),
                    FundingRateAverageThreeDays = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    FundingRateAverageSevenDays = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    FundingRateAverageFourteenDays = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PotentialPositions", x => x.PotentialPositionId);
                });

            migrationBuilder.CreateTable(
                name: "OrderFills",
                columns: table => new
                {
                    OrderFillId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientOrderId = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Size = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    Lot = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    Filled = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    Price = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    Fee = table.Column<decimal>(type: "DECIMAL(12, 6)", nullable: false),
                    FeeCurrency = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    PartInPosition = table.Column<int>(type: "INTEGER", nullable: false),
                    PositionDemandId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderFills", x => x.OrderFillId);
                    table.ForeignKey(
                        name: "FK_OrderFills_PositionDemands_PositionDemandId",
                        column: x => x.PositionDemandId,
                        principalTable: "PositionDemands",
                        principalColumn: "PositionDemandId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderFills_PositionDemandId",
                table: "OrderFills",
                column: "PositionDemandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FundingIncomes");

            migrationBuilder.DropTable(
                name: "OrderFills");

            migrationBuilder.DropTable(
                name: "PositionHistory");

            migrationBuilder.DropTable(
                name: "PotentialPositionRatingHistory");

            migrationBuilder.DropTable(
                name: "PotentialPositions");

            migrationBuilder.DropTable(
                name: "PositionDemands");
        }
    }
}
