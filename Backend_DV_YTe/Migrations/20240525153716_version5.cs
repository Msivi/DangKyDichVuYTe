using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_DV_YTe.Migrations
{
    /// <inheritdoc />
    public partial class version5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ngayTao",
                table: "CTXuatThietBiYTe");

            migrationBuilder.DropColumn(
                name: "ngayTao",
                table: "CTNhapThuoc");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTimes",
                table: "CTXuatThuoc",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTimes",
                table: "CTXuatThietBiYTe",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTimes",
                table: "CTNhapThuoc",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTimes",
                table: "CTNhapThietBiYTe",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTimes",
                table: "CTMuaThuoc",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTimes",
                table: "CTMuaThietBiYTe",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTimes",
                table: "CTBacSi",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateTimes",
                table: "CTXuatThuoc");

            migrationBuilder.DropColumn(
                name: "CreateTimes",
                table: "CTXuatThietBiYTe");

            migrationBuilder.DropColumn(
                name: "CreateTimes",
                table: "CTNhapThuoc");

            migrationBuilder.DropColumn(
                name: "CreateTimes",
                table: "CTNhapThietBiYTe");

            migrationBuilder.DropColumn(
                name: "CreateTimes",
                table: "CTMuaThuoc");

            migrationBuilder.DropColumn(
                name: "CreateTimes",
                table: "CTMuaThietBiYTe");

            migrationBuilder.DropColumn(
                name: "CreateTimes",
                table: "CTBacSi");

            migrationBuilder.AddColumn<DateTime>(
                name: "ngayTao",
                table: "CTXuatThietBiYTe",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ngayTao",
                table: "CTNhapThuoc",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
