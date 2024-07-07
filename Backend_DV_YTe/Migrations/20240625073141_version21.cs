using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_DV_YTe.Migrations
{
    /// <inheritdoc />
    public partial class version21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CTNhapThuoc_Thuoc_MaThuoc",
                table: "CTNhapThuoc");

            migrationBuilder.DropForeignKey(
                name: "FK_KetQuaDichVu_NhanVien_MaNhanVien",
                table: "KetQuaDichVu");

            migrationBuilder.DropIndex(
                name: "IX_KetQuaDichVu_MaNhanVien",
                table: "KetQuaDichVu");

            migrationBuilder.DropColumn(
                name: "MaNhanVien",
                table: "KetQuaDichVu");

            migrationBuilder.AddColumn<int>(
                name: "LoThuocEntityId",
                table: "CTNhapThuoc",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaLoThuoc",
                table: "CTNhapThuoc",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LoThuoc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ngaySanXuat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ngayHetHan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    soLuong = table.Column<int>(type: "int", nullable: false),
                    nhaCungCap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    donGiaBan = table.Column<double>(type: "float", nullable: false),
                    MaThuoc = table.Column<int>(type: "int", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoThuoc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoThuoc_Thuoc_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CTNhapThuoc_LoThuocEntityId",
                table: "CTNhapThuoc",
                column: "LoThuocEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_CTNhapThuoc_MaLoThuoc",
                table: "CTNhapThuoc",
                column: "MaLoThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_LoThuoc_MaThuoc",
                table: "LoThuoc",
                column: "MaThuoc");

            migrationBuilder.AddForeignKey(
                name: "FK_CTNhapThuoc_LoThuoc_LoThuocEntityId",
                table: "CTNhapThuoc",
                column: "LoThuocEntityId",
                principalTable: "LoThuoc",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CTNhapThuoc_LoThuoc_MaLoThuoc",
                table: "CTNhapThuoc",
                column: "MaLoThuoc",
                principalTable: "LoThuoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CTNhapThuoc_Thuoc_MaThuoc",
                table: "CTNhapThuoc",
                column: "MaThuoc",
                principalTable: "Thuoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CTNhapThuoc_LoThuoc_LoThuocEntityId",
                table: "CTNhapThuoc");

            migrationBuilder.DropForeignKey(
                name: "FK_CTNhapThuoc_LoThuoc_MaLoThuoc",
                table: "CTNhapThuoc");

            migrationBuilder.DropForeignKey(
                name: "FK_CTNhapThuoc_Thuoc_MaThuoc",
                table: "CTNhapThuoc");

            migrationBuilder.DropTable(
                name: "LoThuoc");

            migrationBuilder.DropIndex(
                name: "IX_CTNhapThuoc_LoThuocEntityId",
                table: "CTNhapThuoc");

            migrationBuilder.DropIndex(
                name: "IX_CTNhapThuoc_MaLoThuoc",
                table: "CTNhapThuoc");

            migrationBuilder.DropColumn(
                name: "LoThuocEntityId",
                table: "CTNhapThuoc");

            migrationBuilder.DropColumn(
                name: "MaLoThuoc",
                table: "CTNhapThuoc");

            migrationBuilder.AddColumn<int>(
                name: "MaNhanVien",
                table: "KetQuaDichVu",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaDichVu_MaNhanVien",
                table: "KetQuaDichVu",
                column: "MaNhanVien");

            migrationBuilder.AddForeignKey(
                name: "FK_CTNhapThuoc_Thuoc_MaThuoc",
                table: "CTNhapThuoc",
                column: "MaThuoc",
                principalTable: "Thuoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KetQuaDichVu_NhanVien_MaNhanVien",
                table: "KetQuaDichVu",
                column: "MaNhanVien",
                principalTable: "NhanVien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
