using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_DV_YTe.Migrations
{
    /// <inheritdoc />
    public partial class version3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "hinhAnh",
                table: "BacSi",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hinhAnh",
                table: "BacSi");
        }
    }
}
