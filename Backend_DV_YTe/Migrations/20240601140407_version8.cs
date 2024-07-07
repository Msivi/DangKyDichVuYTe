using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_DV_YTe.Migrations
{
    /// <inheritdoc />
    public partial class version8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ThanhToanDV_MaLichHen",
                table: "ThanhToanDV");

            migrationBuilder.AddColumn<string>(
                name: "hinhAnh",
                table: "Thuoc",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hinhAnh",
                table: "ThietBiYTe",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToanDV_MaLichHen",
                table: "ThanhToanDV",
                column: "MaLichHen",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ThanhToanDV_MaLichHen",
                table: "ThanhToanDV");

            migrationBuilder.DropColumn(
                name: "hinhAnh",
                table: "Thuoc");

            migrationBuilder.DropColumn(
                name: "hinhAnh",
                table: "ThietBiYTe");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToanDV_MaLichHen",
                table: "ThanhToanDV",
                column: "MaLichHen");
        }
    }
}
