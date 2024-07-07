using Backend_DV_YTe.Entity;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection.Emit;

namespace Backend_DV_YTe
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        private readonly IConfiguration _configuration;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CTMuaThietBiYTeEntity>()
                .HasKey(c => new { c.MaThietBiYTe, c.MaHoaDon });

            builder.Entity<CTMuaThuocEntity>()
                .HasKey(c => new { c.MaThuoc, c.MaHoaDon });

            builder.Entity<CTNhapThietBiYTeEntity>()
                .HasKey(c => new { c.MaNhapThietBiYTe, c.MaThietBiYTe });

            builder.Entity<CTNhapThuocEntity>()
                .HasKey(c => new { c.MaNhapThuoc, c.MaThuoc });

            builder.Entity<CTXuatThietBiYTeEntity>()
                .HasKey(c => new { c.MaThietBiYTe, c.MaXuatThietBiYTe });

            builder.Entity<CTXuatThuocEntity>()
                .HasKey(c => new { c.MaThuoc, c.MaXuatThuoc });

            builder.Entity<CTBacSiEntity>()
                .HasKey(c => new { c.MaBacSi, c.MaChuyenKhoa });

            builder.Entity<CTNhapThuocEntity>()
                .HasOne(e => e.LoThuoc)
                .WithMany()
                .HasForeignKey(e => e.MaLoThuoc)
                .OnDelete(DeleteBehavior.Restrict); // Prevents cascade delete

            builder.Entity<CTNhapThuocEntity>()
                .HasOne(e => e.Thuoc)
                .WithMany(t => t.CTNhapThuoc)
                .HasForeignKey(e => e.MaThuoc)
                .OnDelete(DeleteBehavior.Restrict); // Prevents cascade delete

            builder.Entity<CTNhapThuocEntity>()
                .HasOne(e => e.NhapThuoc)
                .WithMany(nt => nt.CTNhapThuoc)
                .HasForeignKey(e => e.MaNhapThuoc)
                .OnDelete(DeleteBehavior.Restrict); // Changed to restrict

            builder.Entity<CTNhapThietBiYTeEntity>()
                .HasOne(e => e.LoThietBi)
                .WithMany(l => l.CTNhapThietBiYTe)
                .HasForeignKey(e => e.MaLoThietBi)
                .OnDelete(DeleteBehavior.Restrict); // Prevents cascade delete

            builder.Entity<CTNhapThietBiYTeEntity>()
                .HasOne(e => e.ThietBiYTe)
                .WithMany(t => t.CTNhapThietBiYTe)
                .HasForeignKey(e => e.MaThietBiYTe)
                .OnDelete(DeleteBehavior.Restrict); // Prevents cascade delete

            builder.Entity<CTNhapThietBiYTeEntity>()
                .HasOne(e => e.NhapThietBiYTe)
                .WithMany(nt => nt.CTNhapThietBiYTe)
                .HasForeignKey(e => e.MaNhapThietBiYTe)
                .OnDelete(DeleteBehavior.Restrict); // Changed to restrict

            builder.Entity<ThanhToanDVEntity>()
                .HasIndex(e => e.MaLichHen)
                .IsUnique();

            base.OnModelCreating(builder);
        }


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{

        //    base.OnConfiguring(optionsBuilder);
        //    //optionsBuilder.UseSqlServer("server=SQL8010.site4now.net;database=db_aaa6f4_levi123;uid=db_aaa6f4_levi123_admin;pwd=levi06022002;TrustServerCertificate=True");
        //    //optionsBuilder.UseSqlServer("Data Source = SQL8010.site4now.net; Initial Catalog = db_aaa6f4_levi123; User Id = db_aaa6f4_levi123_admin; Password = levi06022002");

        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<KhachHangEntity> khachHangEntities { get; set; }
        public DbSet<BacSiEntity> BacSiEntities { get; set; }
        public DbSet<CTMuaThietBiYTeEntity> cTMuaThietBiYTeEntities { get; set; }
        public DbSet<CTMuaThuocEntity> cTMuaThuocEntities { get; set; }
        public DbSet<CTNhapThietBiYTeEntity> cTNhapThietBiYTeEntities { get; set; }
        public DbSet<CTNhapThuocEntity> cTNhapThuocEntities { get; set; }
        public DbSet<CTXuatThietBiYTeEntity> cTXuatThietBiYTeEntities { get; set; }
        public DbSet<CTXuatThuocEntity> cTXuatThuocEntities { get; set; }
        public DbSet<DanhGiaEntity> danhGiaEntities { get; set; }
        public DbSet<DichVuEntity> dichVuEntities { get; set; }
        public DbSet<HoaDonEntity> hoaDonEntities { get; set; }
        public DbSet<KetQuaDichVuEntity> ketQuaDichVuEntities { get; set; }
        public DbSet<LichHenEntity> lichHenEntities { get; set; }
        public DbSet<LoaiDichVuEntity> loaiDichVuEntities { get; set; }
        public DbSet<LoaiThuocEntity> loaiThuocEntities { get; set; }
        public DbSet<NhaCungCapEntity> nhaCungCapEntities { get; set; }
        public DbSet<NhanVienEntity> nhanVienEntities { get; set; }
        public DbSet<NhapThietBiYTeEntity> nhapThietBiYTeEntities { get; set; }
        public DbSet<NhapThuocEntity> nhapThuocEntities { get; set; }
        public DbSet<ThanhToanDVEntity> thanhToanDVEntities { get; set; }
        public DbSet<ThietBiYTeEntity> thietBiYTeEntities { get; set; }
        public DbSet<ThuocEntity> thuocEntities { get; set; }
        public DbSet<XuatThietBiYTeEntity> xuatThietBiYTeEntities { get; set; }
        public DbSet<XuatThuocEntity> xuatThuocEntities { get; set; }
        public DbSet<LoaiThietBiEntity> loaiThietBiEntities { get; set; }
        public DbSet<CTBacSiEntity> cTBacSiEntities { get; set; }
        public DbSet<ChuyenKhoaEntity> chuyenKhoaEntities { get; set; }
        public DbSet<DiaChiEntity> diaChiEntities { get; set; }
        public DbSet<LichLamViecEntity> lichLamViecEntities { get; set; }
        public DbSet<LoThuocEntity> loThuocEntities { get; set; }
        public DbSet<LoThietBiYTeEntity> loThietBiYTeEntities { get; set; }
    }
}
