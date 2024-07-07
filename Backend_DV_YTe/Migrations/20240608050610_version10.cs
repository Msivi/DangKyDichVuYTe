using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_DV_YTe.Migrations
{
    /// <inheritdoc />
    public partial class version10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_diaChiEntities_KhachHang_NhanVienmaKhachHang",
                table: "diaChiEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_diaChiEntities",
                table: "diaChiEntities");

            migrationBuilder.DropIndex(
                name: "IX_diaChiEntities_NhanVienmaKhachHang",
                table: "diaChiEntities");

            migrationBuilder.DropColumn(
                name: "NhanVienmaKhachHang",
                table: "diaChiEntities");

            migrationBuilder.RenameTable(
                name: "diaChiEntities",
                newName: "DiaChi");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiaChi",
                table: "DiaChi",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DiaChi_KhachHangId",
                table: "DiaChi",
                column: "KhachHangId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiaChi_KhachHang_KhachHangId",
                table: "DiaChi",
                column: "KhachHangId",
                principalTable: "KhachHang",
                principalColumn: "maKhachHang",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiaChi_KhachHang_KhachHangId",
                table: "DiaChi");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiaChi",
                table: "DiaChi");

            migrationBuilder.DropIndex(
                name: "IX_DiaChi_KhachHangId",
                table: "DiaChi");

            migrationBuilder.RenameTable(
                name: "DiaChi",
                newName: "diaChiEntities");

            //migrationBuilder.AddColumn<int>(
            //    name: "NhanVienmaKhachHang",
            //    table: "diaChiEntities",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_diaChiEntities",
                table: "diaChiEntities",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_diaChiEntities_NhanVienmaKhachHang",
                table: "diaChiEntities",
                column: "NhanVienmaKhachHang");

            migrationBuilder.AddForeignKey(
                name: "FK_diaChiEntities_KhachHang_NhanVienmaKhachHang",
                table: "diaChiEntities",
                column: "NhanVienmaKhachHang",
                principalTable: "KhachHang",
                principalColumn: "maKhachHang",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
