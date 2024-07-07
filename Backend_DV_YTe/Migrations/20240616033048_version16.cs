using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_DV_YTe.Migrations
{
    /// <inheritdoc />
    public partial class version16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ghiChu",
                table: "ThanhToanDV");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "BacSi",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "matKhau",
                table: "BacSi",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "BacSi");

            migrationBuilder.DropColumn(
                name: "matKhau",
                table: "BacSi");

            migrationBuilder.AddColumn<string>(
                name: "ghiChu",
                table: "ThanhToanDV",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
