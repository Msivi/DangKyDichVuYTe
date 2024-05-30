using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_DV_YTe.Migrations
{
    /// <inheritdoc />
    public partial class version7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CTMuaThuoc_NhanVien_NhanVienEntityId",
                table: "CTMuaThuoc");

            migrationBuilder.DropIndex(
                name: "IX_CTMuaThuoc_NhanVienEntityId",
                table: "CTMuaThuoc");

            migrationBuilder.DropColumn(
                name: "NhanVienEntityId",
                table: "CTMuaThuoc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NhanVienEntityId",
                table: "CTMuaThuoc",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CTMuaThuoc_NhanVienEntityId",
                table: "CTMuaThuoc",
                column: "NhanVienEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_CTMuaThuoc_NhanVien_NhanVienEntityId",
                table: "CTMuaThuoc",
                column: "NhanVienEntityId",
                principalTable: "NhanVien",
                principalColumn: "Id");
        }
    }
}
