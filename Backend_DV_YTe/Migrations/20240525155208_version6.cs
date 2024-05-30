using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_DV_YTe.Migrations
{
    /// <inheritdoc />
    public partial class version6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_XuatThietBiYTe_NhanVien_MaNhanVien",
                table: "XuatThietBiYTe");

            migrationBuilder.DropForeignKey(
                name: "FK_XuatThuoc_NhanVien_MaNhanVien",
                table: "XuatThuoc");

            migrationBuilder.DropIndex(
                name: "IX_XuatThuoc_MaNhanVien",
                table: "XuatThuoc");

            migrationBuilder.DropIndex(
                name: "IX_XuatThietBiYTe_MaNhanVien",
                table: "XuatThietBiYTe");

            migrationBuilder.DropColumn(
                name: "MaNhanVien",
                table: "XuatThuoc");

            migrationBuilder.DropColumn(
                name: "MaNhanVien",
                table: "XuatThietBiYTe");

            migrationBuilder.DropColumn(
                name: "ngayTao",
                table: "CTXuatThuoc");

            migrationBuilder.DropColumn(
                name: "ngayTao",
                table: "CTNhapThietBiYTe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaNhanVien",
                table: "XuatThuoc",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaNhanVien",
                table: "XuatThietBiYTe",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ngayTao",
                table: "CTXuatThuoc",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ngayTao",
                table: "CTNhapThietBiYTe",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_XuatThuoc_MaNhanVien",
                table: "XuatThuoc",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_XuatThietBiYTe_MaNhanVien",
                table: "XuatThietBiYTe",
                column: "MaNhanVien");

            migrationBuilder.AddForeignKey(
                name: "FK_XuatThietBiYTe_NhanVien_MaNhanVien",
                table: "XuatThietBiYTe",
                column: "MaNhanVien",
                principalTable: "NhanVien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_XuatThuoc_NhanVien_MaNhanVien",
                table: "XuatThuoc",
                column: "MaNhanVien",
                principalTable: "NhanVien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
