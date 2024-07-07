using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_DV_YTe.Migrations
{
    /// <inheritdoc />
    public partial class version25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HuyLoThietBiYTe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoThietBiId = table.Column<int>(type: "int", nullable: false),
                    NgayHuy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LyDoHuy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HuyLoThietBiYTe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HuyLoThietBiYTe_LoThietBiYTe_LoThietBiId",
                        column: x => x.LoThietBiId,
                        principalTable: "LoThietBiYTe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HuyLoThuoc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoThuocId = table.Column<int>(type: "int", nullable: false),
                    NgayHuy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LyDoHuy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HuyLoThuoc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HuyLoThuoc_LoThuoc_LoThuocId",
                        column: x => x.LoThuocId,
                        principalTable: "LoThuoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HuyLoThietBiYTe_LoThietBiId",
                table: "HuyLoThietBiYTe",
                column: "LoThietBiId");

            migrationBuilder.CreateIndex(
                name: "IX_HuyLoThuoc_LoThuocId",
                table: "HuyLoThuoc",
                column: "LoThuocId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HuyLoThietBiYTe");

            migrationBuilder.DropTable(
                name: "HuyLoThuoc");
        }
    }
}
