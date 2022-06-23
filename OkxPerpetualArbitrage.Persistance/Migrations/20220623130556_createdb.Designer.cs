﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OkxPerpetualArbitrage.Persistance;

#nullable disable

namespace OkxPerpetualArbitrage.Persistance.Migrations
{
    [DbContext(typeof(SpotPerpDbContext))]
    [Migration("20220623130556_createdb")]
    partial class createdb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.6");

            modelBuilder.Entity("OkxPerpetualArbitrage.Domain.Entities.FundingIncome", b =>
                {
                    b.Property<int>("FundingIncomeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Amount")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<string>("BillId")
                        .IsRequired()
                        .HasColumnType("VARCHAR(50)");

                    b.Property<bool>("IsInCurrentPosition")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("VARCHAR(10)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("DATETIME");

                    b.HasKey("FundingIncomeId");

                    b.ToTable("FundingIncomes");
                });

            modelBuilder.Entity("OkxPerpetualArbitrage.Domain.Entities.OrderFill", b =>
                {
                    b.Property<int>("OrderFillId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClientOrderId")
                        .IsRequired()
                        .HasColumnType("VARCHAR(50)");

                    b.Property<decimal>("Fee")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<string>("FeeCurrency")
                        .IsRequired()
                        .HasColumnType("VARCHAR(50)");

                    b.Property<decimal>("Filled")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("Lot")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<int>("PartInPosition")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PositionDemandId")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Price")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("Size")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("DATETIME");

                    b.HasKey("OrderFillId");

                    b.HasIndex("PositionDemandId");

                    b.ToTable("OrderFills");
                });

            modelBuilder.Entity("OkxPerpetualArbitrage.Domain.Entities.PositionDemand", b =>
                {
                    b.Property<int>("PositionDemandId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("ActualSpread")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("Filled")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<bool>("IsCanceled")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsInstant")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("OpenDate")
                        .HasColumnType("DATETIME");

                    b.Property<int>("PositionDemandSide")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PositionDemandState")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Spread")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("VARCHAR(10)");

                    b.Property<decimal>("TotalSize")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("DATETIME");

                    b.HasKey("PositionDemandId");

                    b.ToTable("PositionDemands");
                });

            modelBuilder.Entity("OkxPerpetualArbitrage.Domain.Entities.PositionHistory", b =>
                {
                    b.Property<int>("PositionHistoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CloseDate")
                        .HasColumnType("DATETIME");

                    b.Property<decimal>("Fee")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("Funding")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<DateTime>("OpenDate")
                        .HasColumnType("DATETIME");

                    b.Property<decimal>("PerpTrade")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("SpotTrade")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("VARCHAR(10)");

                    b.Property<decimal>("TotalPNL")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.HasKey("PositionHistoryId");

                    b.ToTable("PositionHistory");
                });

            modelBuilder.Entity("OkxPerpetualArbitrage.Domain.Entities.PotentialPosition", b =>
                {
                    b.Property<int>("PotentialPositionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("CloseSpread")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("ContractValuePerp")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("FeePerpMaker")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("FeePerpTaker")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("FeeSpotMaker")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("FeeSpotTaker")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("Funding")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("FundingRateAverageFourteenDays")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("FundingRateAverageSevenDays")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("FundingRateAverageThreeDays")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<bool>("IsUpToDate")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("LotSizeSpot")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("MarkPrice")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("MinSizePerp")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("MinSizeSpot")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("NextFunding")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("Rating")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("Spread")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("VARCHAR(10)");

                    b.Property<decimal>("TickSizePerp")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<decimal>("TickSizeSpot")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.HasKey("PotentialPositionId");

                    b.ToTable("PotentialPositions");
                });

            modelBuilder.Entity("OkxPerpetualArbitrage.Domain.Entities.PotentialPositionRatingHistory", b =>
                {
                    b.Property<int>("PotentialPositionRatingHistoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Rating")
                        .HasColumnType("DECIMAL(12, 6)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("VARCHAR(10)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("DATETIME");

                    b.HasKey("PotentialPositionRatingHistoryId");

                    b.ToTable("PotentialPositionRatingHistory");
                });

            modelBuilder.Entity("OkxPerpetualArbitrage.Domain.Entities.OrderFill", b =>
                {
                    b.HasOne("OkxPerpetualArbitrage.Domain.Entities.PositionDemand", "PositionDemand")
                        .WithMany()
                        .HasForeignKey("PositionDemandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PositionDemand");
                });
#pragma warning restore 612, 618
        }
    }
}
