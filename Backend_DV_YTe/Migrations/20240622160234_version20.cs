using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_DV_YTe.Migrations
{
    /// <inheritdoc />
    public partial class version20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichLamViecEntity_BacSi_MaBacSi",
                table: "LichLamViecEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LichLamViecEntity",
                table: "LichLamViecEntity");

            migrationBuilder.RenameTable(
                name: "LichLamViecEntity",
                newName: "LichLamViec");

            migrationBuilder.RenameIndex(
                name: "IX_LichLamViecEntity_MaBacSi",
                table: "LichLamViec",
                newName: "IX_LichLamViec_MaBacSi");

            migrationBuilder.AlterColumn<DateTime>(
                name: "GioKetThuc",
                table: "LichLamViec",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<DateTime>(
                name: "GioBatDau",
                table: "LichLamViec",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LichLamViec",
                table: "LichLamViec",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LichLamViec_BacSi_MaBacSi",
                table: "LichLamViec",
                column: "MaBacSi",
                principalTable: "BacSi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichLamViec_BacSi_MaBacSi",
                table: "LichLamViec");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LichLamViec",
                table: "LichLamViec");

            migrationBuilder.RenameTable(
                name: "LichLamViec",
                newName: "LichLamViecEntity");

            migrationBuilder.RenameIndex(
                name: "IX_LichLamViec_MaBacSi",
                table: "LichLamViecEntity",
                newName: "IX_LichLamViecEntity_MaBacSi");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "GioKetThuc",
                table: "LichLamViecEntity",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "GioBatDau",
                table: "LichLamViecEntity",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LichLamViecEntity",
                table: "LichLamViecEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LichLamViecEntity_BacSi_MaBacSi",
                table: "LichLamViecEntity",
                column: "MaBacSi",
                principalTable: "BacSi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
