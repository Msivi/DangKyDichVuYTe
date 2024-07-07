using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_DV_YTe.Migrations
{
    /// <inheritdoc />
    public partial class version13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "tongTien",
                table: "NhapThuoc",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "tongTien",
                table: "NhapThietBiYTe",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ghiChu",
                table: "LichHen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ghiChu",
                table: "HoaDon",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "donGiaNhap",
                table: "CTNhapThuoc",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "donGiaNhap",
                table: "CTNhapThietBiYTe",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tongTien",
                table: "NhapThuoc");

            migrationBuilder.DropColumn(
                name: "tongTien",
                table: "NhapThietBiYTe");

            migrationBuilder.DropColumn(
                name: "ghiChu",
                table: "LichHen");

            migrationBuilder.DropColumn(
                name: "ghiChu",
                table: "HoaDon");

            migrationBuilder.DropColumn(
                name: "donGiaNhap",
                table: "CTNhapThuoc");

            migrationBuilder.DropColumn(
                name: "donGiaNhap",
                table: "CTNhapThietBiYTe");
        }
    }
}
