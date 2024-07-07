using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_DV_YTe.Migrations
{
    /// <inheritdoc />
    public partial class version24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CTNhapThietBiYTe_LoThuoc_MaLoThuoc",
                table: "CTNhapThietBiYTe");

            migrationBuilder.DropForeignKey(
                name: "FK_CTNhapThietBiYTe_NhapThietBiYTe_MaNhapThietBiYTe",
                table: "CTNhapThietBiYTe");

            migrationBuilder.DropForeignKey(
                name: "FK_CTNhapThietBiYTe_ThietBiYTe_MaThietBiYTe",
                table: "CTNhapThietBiYTe");

            migrationBuilder.DropForeignKey(
                name: "FK_CTNhapThuoc_NhapThuoc_MaNhapThuoc",
                table: "CTNhapThuoc");

            migrationBuilder.RenameColumn(
                name: "MaLoThuoc",
                table: "CTNhapThietBiYTe",
                newName: "MaLoThietBi");

            migrationBuilder.RenameIndex(
                name: "IX_CTNhapThietBiYTe_MaLoThuoc",
                table: "CTNhapThietBiYTe",
                newName: "IX_CTNhapThietBiYTe_MaLoThietBi");

            migrationBuilder.CreateTable(
                name: "LoThietBiYTe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ngaySanXuat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ngayHetHan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    soLuong = table.Column<int>(type: "int", nullable: false),
                    nhaCungCap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    donGiaBan = table.Column<double>(type: "float", nullable: false),
                    MaThietbiYTe = table.Column<int>(type: "int", nullable: true),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoThietBiYTe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoThietBiYTe_ThietBiYTe_MaThietbiYTe",
                        column: x => x.MaThietbiYTe,
                        principalTable: "ThietBiYTe",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoThietBiYTe_MaThietbiYTe",
                table: "LoThietBiYTe",
                column: "MaThietbiYTe");

            migrationBuilder.AddForeignKey(
                name: "FK_CTNhapThietBiYTe_LoThietBiYTe_MaLoThietBi",
                table: "CTNhapThietBiYTe",
                column: "MaLoThietBi",
                principalTable: "LoThietBiYTe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CTNhapThietBiYTe_NhapThietBiYTe_MaNhapThietBiYTe",
                table: "CTNhapThietBiYTe",
                column: "MaNhapThietBiYTe",
                principalTable: "NhapThietBiYTe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CTNhapThietBiYTe_ThietBiYTe_MaThietBiYTe",
                table: "CTNhapThietBiYTe",
                column: "MaThietBiYTe",
                principalTable: "ThietBiYTe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CTNhapThuoc_NhapThuoc_MaNhapThuoc",
                table: "CTNhapThuoc",
                column: "MaNhapThuoc",
                principalTable: "NhapThuoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CTNhapThietBiYTe_LoThietBiYTe_MaLoThietBi",
                table: "CTNhapThietBiYTe");

            migrationBuilder.DropForeignKey(
                name: "FK_CTNhapThietBiYTe_NhapThietBiYTe_MaNhapThietBiYTe",
                table: "CTNhapThietBiYTe");

            migrationBuilder.DropForeignKey(
                name: "FK_CTNhapThietBiYTe_ThietBiYTe_MaThietBiYTe",
                table: "CTNhapThietBiYTe");

            migrationBuilder.DropForeignKey(
                name: "FK_CTNhapThuoc_NhapThuoc_MaNhapThuoc",
                table: "CTNhapThuoc");

            migrationBuilder.DropTable(
                name: "LoThietBiYTe");

            migrationBuilder.RenameColumn(
                name: "MaLoThietBi",
                table: "CTNhapThietBiYTe",
                newName: "MaLoThuoc");

            migrationBuilder.RenameIndex(
                name: "IX_CTNhapThietBiYTe_MaLoThietBi",
                table: "CTNhapThietBiYTe",
                newName: "IX_CTNhapThietBiYTe_MaLoThuoc");

            migrationBuilder.AddForeignKey(
                name: "FK_CTNhapThietBiYTe_LoThuoc_MaLoThuoc",
                table: "CTNhapThietBiYTe",
                column: "MaLoThuoc",
                principalTable: "LoThuoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTNhapThietBiYTe_NhapThietBiYTe_MaNhapThietBiYTe",
                table: "CTNhapThietBiYTe",
                column: "MaNhapThietBiYTe",
                principalTable: "NhapThietBiYTe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTNhapThietBiYTe_ThietBiYTe_MaThietBiYTe",
                table: "CTNhapThietBiYTe",
                column: "MaThietBiYTe",
                principalTable: "ThietBiYTe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTNhapThuoc_NhapThuoc_MaNhapThuoc",
                table: "CTNhapThuoc",
                column: "MaNhapThuoc",
                principalTable: "NhapThuoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
