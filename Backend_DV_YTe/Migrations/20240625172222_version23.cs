using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_DV_YTe.Migrations
{
    /// <inheritdoc />
    public partial class version23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "soLuong",
                table: "Thuoc");

            migrationBuilder.DropColumn(
                name: "ngayHetHan",
                table: "ThietBiYTe");

            migrationBuilder.DropColumn(
                name: "ngaySanXuat",
                table: "ThietBiYTe");

            migrationBuilder.DropColumn(
                name: "soLuong",
                table: "ThietBiYTe");

            migrationBuilder.AddColumn<int>(
                name: "ThietBiYTeEntityId",
                table: "LoThuoc",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaLoThuoc",
                table: "CTNhapThietBiYTe",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LoThuoc_ThietBiYTeEntityId",
                table: "LoThuoc",
                column: "ThietBiYTeEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_CTNhapThietBiYTe_MaLoThuoc",
                table: "CTNhapThietBiYTe",
                column: "MaLoThuoc");

            migrationBuilder.AddForeignKey(
                name: "FK_CTNhapThietBiYTe_LoThuoc_MaLoThuoc",
                table: "CTNhapThietBiYTe",
                column: "MaLoThuoc",
                principalTable: "LoThuoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoThuoc_ThietBiYTe_ThietBiYTeEntityId",
                table: "LoThuoc",
                column: "ThietBiYTeEntityId",
                principalTable: "ThietBiYTe",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CTNhapThietBiYTe_LoThuoc_MaLoThuoc",
                table: "CTNhapThietBiYTe");

            migrationBuilder.DropForeignKey(
                name: "FK_LoThuoc_ThietBiYTe_ThietBiYTeEntityId",
                table: "LoThuoc");

            migrationBuilder.DropIndex(
                name: "IX_LoThuoc_ThietBiYTeEntityId",
                table: "LoThuoc");

            migrationBuilder.DropIndex(
                name: "IX_CTNhapThietBiYTe_MaLoThuoc",
                table: "CTNhapThietBiYTe");

            migrationBuilder.DropColumn(
                name: "ThietBiYTeEntityId",
                table: "LoThuoc");

            migrationBuilder.DropColumn(
                name: "MaLoThuoc",
                table: "CTNhapThietBiYTe");

            migrationBuilder.AddColumn<int>(
                name: "soLuong",
                table: "Thuoc",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ngayHetHan",
                table: "ThietBiYTe",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ngaySanXuat",
                table: "ThietBiYTe",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "soLuong",
                table: "ThietBiYTe",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
