using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_DV_YTe.Migrations
{
    /// <inheritdoc />
    public partial class version1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BacSi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenBacSi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bangCap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacSi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChuyenKhoa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenChuyenKhoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuyenKhoa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    maKhachHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenKhachHang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    matKhau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CMND = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.maKhachHang);
                });

            migrationBuilder.CreateTable(
                name: "LoaiDichVu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenLoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiDichVu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoaiThietBi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenLoaiThietBi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiThietBi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoaiThuoc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenLoaiThuoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiThuoc", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NhaCungCap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenNhaCungCap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    diaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaCungCap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenNhanVien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    matKhau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CMND = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVien", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CTBacSi",
                columns: table => new
                {
                    MaBacSi = table.Column<int>(type: "int", nullable: false),
                    MaChuyenKhoa = table.Column<int>(type: "int", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTBacSi", x => new { x.MaBacSi, x.MaChuyenKhoa });
                    table.ForeignKey(
                        name: "FK_CTBacSi_BacSi_MaBacSi",
                        column: x => x.MaBacSi,
                        principalTable: "BacSi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CTBacSi_ChuyenKhoa_MaChuyenKhoa",
                        column: x => x.MaChuyenKhoa,
                        principalTable: "ChuyenKhoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ngayMua = table.Column<DateTime>(type: "datetime2", nullable: false),
                    tongTien = table.Column<double>(type: "float", nullable: false),
                    trangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoaDon_KhachHang_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "maKhachHang",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DichVu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenDichVu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    moTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gia = table.Column<double>(type: "float", nullable: false),
                    MaLoaiDichVu = table.Column<int>(type: "int", nullable: false),
                    MaChuyenKhoa = table.Column<int>(type: "int", nullable: true),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DichVu_ChuyenKhoa_MaChuyenKhoa",
                        column: x => x.MaChuyenKhoa,
                        principalTable: "ChuyenKhoa",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DichVu_LoaiDichVu_MaLoaiDichVu",
                        column: x => x.MaLoaiDichVu,
                        principalTable: "LoaiDichVu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThietBiYTe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenThietBi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    donGia = table.Column<double>(type: "float", nullable: false),
                    soLuong = table.Column<int>(type: "int", nullable: false),
                    ngaySanXuat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ngayHetHan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    nhaSanXuat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    moTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaLoaiThietBi = table.Column<int>(type: "int", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThietBiYTe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThietBiYTe_LoaiThietBi_MaLoaiThietBi",
                        column: x => x.MaLoaiThietBi,
                        principalTable: "LoaiThietBi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Thuoc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenThuoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    donViTinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    donGia = table.Column<double>(type: "float", nullable: false),
                    soLuong = table.Column<int>(type: "int", nullable: false),
                    ngaySanXuat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ngayHetHan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    nhaSanXuat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    thanhPhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    moTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaLoaiThuoc = table.Column<int>(type: "int", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thuoc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Thuoc_LoaiThuoc_MaLoaiThuoc",
                        column: x => x.MaLoaiThuoc,
                        principalTable: "LoaiThuoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhapThietBiYTe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ngayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    MaNhaCungCap = table.Column<int>(type: "int", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhapThietBiYTe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NhapThietBiYTe_NhaCungCap_MaNhaCungCap",
                        column: x => x.MaNhaCungCap,
                        principalTable: "NhaCungCap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NhapThietBiYTe_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhapThuoc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ngayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    MaNhaCungCap = table.Column<int>(type: "int", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhapThuoc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NhapThuoc_NhaCungCap_MaNhaCungCap",
                        column: x => x.MaNhaCungCap,
                        principalTable: "NhaCungCap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NhapThuoc_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "XuatThietBiYTe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ngayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XuatThietBiYTe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_XuatThietBiYTe_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "XuatThuoc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ngayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XuatThuoc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_XuatThuoc_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichHen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    diaDiem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    trangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    thoiGianDuKien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaBacSi = table.Column<int>(type: "int", nullable: false),
                    MaDichVu = table.Column<int>(type: "int", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichHen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LichHen_BacSi_MaBacSi",
                        column: x => x.MaBacSi,
                        principalTable: "BacSi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichHen_DichVu_MaDichVu",
                        column: x => x.MaDichVu,
                        principalTable: "DichVu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichHen_KhachHang_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "maKhachHang",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CTMuaThietBiYTe",
                columns: table => new
                {
                    MaHoaDon = table.Column<int>(type: "int", nullable: false),
                    MaThietBiYTe = table.Column<int>(type: "int", nullable: false),
                    soLuong = table.Column<int>(type: "int", nullable: false),
                    donGia = table.Column<double>(type: "float", nullable: false),
                    thanhTien = table.Column<double>(type: "float", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTMuaThietBiYTe", x => new { x.MaThietBiYTe, x.MaHoaDon });
                    table.ForeignKey(
                        name: "FK_CTMuaThietBiYTe_HoaDon_MaHoaDon",
                        column: x => x.MaHoaDon,
                        principalTable: "HoaDon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CTMuaThietBiYTe_ThietBiYTe_MaThietBiYTe",
                        column: x => x.MaThietBiYTe,
                        principalTable: "ThietBiYTe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CTMuaThuoc",
                columns: table => new
                {
                    MaHoaDon = table.Column<int>(type: "int", nullable: false),
                    MaThuoc = table.Column<int>(type: "int", nullable: false),
                    soLuong = table.Column<int>(type: "int", nullable: false),
                    donGia = table.Column<double>(type: "float", nullable: false),
                    thanhTien = table.Column<double>(type: "float", nullable: false),
                    NhanVienEntityId = table.Column<int>(type: "int", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTMuaThuoc", x => new { x.MaThuoc, x.MaHoaDon });
                    table.ForeignKey(
                        name: "FK_CTMuaThuoc_HoaDon_MaHoaDon",
                        column: x => x.MaHoaDon,
                        principalTable: "HoaDon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CTMuaThuoc_NhanVien_NhanVienEntityId",
                        column: x => x.NhanVienEntityId,
                        principalTable: "NhanVien",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CTMuaThuoc_Thuoc_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CTNhapThietBiYTe",
                columns: table => new
                {
                    MaThietBiYTe = table.Column<int>(type: "int", nullable: false),
                    MaNhapThietBiYTe = table.Column<int>(type: "int", nullable: false),
                    soLuong = table.Column<int>(type: "int", nullable: false),
                    ngayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTNhapThietBiYTe", x => new { x.MaNhapThietBiYTe, x.MaThietBiYTe });
                    table.ForeignKey(
                        name: "FK_CTNhapThietBiYTe_NhapThietBiYTe_MaNhapThietBiYTe",
                        column: x => x.MaNhapThietBiYTe,
                        principalTable: "NhapThietBiYTe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CTNhapThietBiYTe_ThietBiYTe_MaThietBiYTe",
                        column: x => x.MaThietBiYTe,
                        principalTable: "ThietBiYTe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CTNhapThuoc",
                columns: table => new
                {
                    MaThuoc = table.Column<int>(type: "int", nullable: false),
                    MaNhapThuoc = table.Column<int>(type: "int", nullable: false),
                    soLuong = table.Column<int>(type: "int", nullable: false),
                    ngayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTNhapThuoc", x => new { x.MaNhapThuoc, x.MaThuoc });
                    table.ForeignKey(
                        name: "FK_CTNhapThuoc_NhapThuoc_MaNhapThuoc",
                        column: x => x.MaNhapThuoc,
                        principalTable: "NhapThuoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CTNhapThuoc_Thuoc_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CTXuatThietBiYTe",
                columns: table => new
                {
                    MaXuatThietBiYTe = table.Column<int>(type: "int", nullable: false),
                    MaThietBiYTe = table.Column<int>(type: "int", nullable: false),
                    soLuong = table.Column<int>(type: "int", nullable: false),
                    ngayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTXuatThietBiYTe", x => new { x.MaThietBiYTe, x.MaXuatThietBiYTe });
                    table.ForeignKey(
                        name: "FK_CTXuatThietBiYTe_ThietBiYTe_MaThietBiYTe",
                        column: x => x.MaThietBiYTe,
                        principalTable: "ThietBiYTe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CTXuatThietBiYTe_XuatThietBiYTe_MaXuatThietBiYTe",
                        column: x => x.MaXuatThietBiYTe,
                        principalTable: "XuatThietBiYTe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CTXuatThuoc",
                columns: table => new
                {
                    MaThuoc = table.Column<int>(type: "int", nullable: false),
                    MaXuatThuoc = table.Column<int>(type: "int", nullable: false),
                    soLuong = table.Column<int>(type: "int", nullable: false),
                    ngayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTXuatThuoc", x => new { x.MaThuoc, x.MaXuatThuoc });
                    table.ForeignKey(
                        name: "FK_CTXuatThuoc_Thuoc_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CTXuatThuoc_XuatThuoc_MaXuatThuoc",
                        column: x => x.MaXuatThuoc,
                        principalTable: "XuatThuoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KetQuaDichVu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    moTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaLichHen = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaDichVu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KetQuaDichVu_LichHen_MaLichHen",
                        column: x => x.MaLichHen,
                        principalTable: "LichHen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KetQuaDichVu_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThanhToanDV",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tongTien = table.Column<double>(type: "float", nullable: false),
                    ngayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    phuongThucThanhToan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    trangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaLichHen = table.Column<int>(type: "int", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToanDV", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThanhToanDV_LichHen_MaLichHen",
                        column: x => x.MaLichHen,
                        principalTable: "LichHen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhGia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    noiDungDanhGia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    soSaoDanhGia = table.Column<double>(type: "float", nullable: false),
                    trangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaKetQuaDichVu = table.Column<int>(type: "int", nullable: false),
                    CreateTimes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhGia_KetQuaDichVu_MaKetQuaDichVu",
                        column: x => x.MaKetQuaDichVu,
                        principalTable: "KetQuaDichVu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CTBacSi_MaChuyenKhoa",
                table: "CTBacSi",
                column: "MaChuyenKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_CTMuaThietBiYTe_MaHoaDon",
                table: "CTMuaThietBiYTe",
                column: "MaHoaDon");

            migrationBuilder.CreateIndex(
                name: "IX_CTMuaThuoc_MaHoaDon",
                table: "CTMuaThuoc",
                column: "MaHoaDon");

            migrationBuilder.CreateIndex(
                name: "IX_CTMuaThuoc_NhanVienEntityId",
                table: "CTMuaThuoc",
                column: "NhanVienEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_CTNhapThietBiYTe_MaThietBiYTe",
                table: "CTNhapThietBiYTe",
                column: "MaThietBiYTe");

            migrationBuilder.CreateIndex(
                name: "IX_CTNhapThuoc_MaThuoc",
                table: "CTNhapThuoc",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_CTXuatThietBiYTe_MaXuatThietBiYTe",
                table: "CTXuatThietBiYTe",
                column: "MaXuatThietBiYTe");

            migrationBuilder.CreateIndex(
                name: "IX_CTXuatThuoc_MaXuatThuoc",
                table: "CTXuatThuoc",
                column: "MaXuatThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_MaKetQuaDichVu",
                table: "DanhGia",
                column: "MaKetQuaDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_DichVu_MaChuyenKhoa",
                table: "DichVu",
                column: "MaChuyenKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_DichVu_MaLoaiDichVu",
                table: "DichVu",
                column: "MaLoaiDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaKhachHang",
                table: "HoaDon",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaDichVu_MaLichHen",
                table: "KetQuaDichVu",
                column: "MaLichHen");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaDichVu_MaNhanVien",
                table: "KetQuaDichVu",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_MaBacSi",
                table: "LichHen",
                column: "MaBacSi");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_MaDichVu",
                table: "LichHen",
                column: "MaDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_MaKhachHang",
                table: "LichHen",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_NhapThietBiYTe_MaNhaCungCap",
                table: "NhapThietBiYTe",
                column: "MaNhaCungCap");

            migrationBuilder.CreateIndex(
                name: "IX_NhapThietBiYTe_MaNhanVien",
                table: "NhapThietBiYTe",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_NhapThuoc_MaNhaCungCap",
                table: "NhapThuoc",
                column: "MaNhaCungCap");

            migrationBuilder.CreateIndex(
                name: "IX_NhapThuoc_MaNhanVien",
                table: "NhapThuoc",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToanDV_MaLichHen",
                table: "ThanhToanDV",
                column: "MaLichHen");

            migrationBuilder.CreateIndex(
                name: "IX_ThietBiYTe_MaLoaiThietBi",
                table: "ThietBiYTe",
                column: "MaLoaiThietBi");

            migrationBuilder.CreateIndex(
                name: "IX_Thuoc_MaLoaiThuoc",
                table: "Thuoc",
                column: "MaLoaiThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_XuatThietBiYTe_MaNhanVien",
                table: "XuatThietBiYTe",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_XuatThuoc_MaNhanVien",
                table: "XuatThuoc",
                column: "MaNhanVien");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CTBacSi");

            migrationBuilder.DropTable(
                name: "CTMuaThietBiYTe");

            migrationBuilder.DropTable(
                name: "CTMuaThuoc");

            migrationBuilder.DropTable(
                name: "CTNhapThietBiYTe");

            migrationBuilder.DropTable(
                name: "CTNhapThuoc");

            migrationBuilder.DropTable(
                name: "CTXuatThietBiYTe");

            migrationBuilder.DropTable(
                name: "CTXuatThuoc");

            migrationBuilder.DropTable(
                name: "DanhGia");

            migrationBuilder.DropTable(
                name: "ThanhToanDV");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "NhapThietBiYTe");

            migrationBuilder.DropTable(
                name: "NhapThuoc");

            migrationBuilder.DropTable(
                name: "ThietBiYTe");

            migrationBuilder.DropTable(
                name: "XuatThietBiYTe");

            migrationBuilder.DropTable(
                name: "Thuoc");

            migrationBuilder.DropTable(
                name: "XuatThuoc");

            migrationBuilder.DropTable(
                name: "KetQuaDichVu");

            migrationBuilder.DropTable(
                name: "NhaCungCap");

            migrationBuilder.DropTable(
                name: "LoaiThietBi");

            migrationBuilder.DropTable(
                name: "LoaiThuoc");

            migrationBuilder.DropTable(
                name: "LichHen");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "BacSi");

            migrationBuilder.DropTable(
                name: "DichVu");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "ChuyenKhoa");

            migrationBuilder.DropTable(
                name: "LoaiDichVu");
        }
    }
}
