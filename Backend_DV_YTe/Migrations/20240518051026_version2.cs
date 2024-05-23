using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_DV_YTe.Migrations
{
    /// <inheritdoc />
    public partial class version2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GioiTinh",
                table: "KhachHang",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgaySinh",
                table: "KhachHang",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hinhAnh",
                table: "DichVu",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GioiTinh",
                table: "KhachHang");

            migrationBuilder.DropColumn(
                name: "NgaySinh",
                table: "KhachHang");

            migrationBuilder.DropColumn(
                name: "hinhAnh",
                table: "DichVu");
        }
    }
}
