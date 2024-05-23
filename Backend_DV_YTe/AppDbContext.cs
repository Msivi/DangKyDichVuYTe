using Backend_DV_YTe.Entity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Backend_DV_YTe
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CTMuaThietBiYTeEntity>()
           .HasKey(c => new { c.MaThietBiYTe, c.MaHoaDon });

            builder.Entity<CTMuaThuocEntity>()
          .HasKey(c => new { c.MaThuoc, c.MaHoaDon });

            builder.Entity<CTNhapThietBiYTeEntity>()
          .HasKey(c => new { c.MaNhapThietBiYTe, c.MaThietBiYTe});

            builder.Entity<CTNhapThuocEntity>()
          .HasKey(c => new { c.MaNhapThuoc, c.MaThuoc });

            builder.Entity<CTXuatThietBiYTeEntity>()
          .HasKey(c => new { c.MaThietBiYTe, c.MaXuatThietBiYTe});

            builder.Entity<CTXuatThuocEntity>()
          .HasKey(c => new { c.MaThuoc, c.MaXuatThuoc });

            builder.Entity<CTBacSiEntity>()
          .HasKey(c => new { c.MaBacSi, c.MaChuyenKhoa });

            //builder.Entity<KetQuaDichVuEntity>()
            //  .HasOne(k => k.LichHen)
            //  .WithOne(l => l.KetQuaDichVu)
            //  .HasForeignKey<LichHenEntity>(l => l.MaDichVu);

            //builder.Entity<LichHenEntity>()
            // .HasOne(l => l.ThanhToanDV)
            // .WithOne(t => t.LichHen)
            // .HasForeignKey<ThanhToanDVEntity>(t => t.Id);

            //builder.Entity<DanhGiaEntity>()
            //.HasOne(d => d.NhanVien)
            //.WithMany(n => n.DanhGia)
            //.HasForeignKey(d => d.MaNhanVien)
            //.OnDelete(DeleteBehavior.NoAction);

            //builder.Entity<LichHenEntity>()
            //.HasOne(l => l.KetQuaDichVu)
            //.WithOne(k => k.LichHen)
            //.HasForeignKey<LichHenEntity>(l => l.MaDichVu)
            //.OnDelete(DeleteBehavior.NoAction);
            //============================
            //    builder.Entity<ThietBiYTeEntity>()
            //.HasOne<LoaiThietBiEntity>(t => t.LoaiThietBi)
            //.WithMany(l => l.LoaiThietBi)
            //.HasForeignKey(t => t.MaLoaiThietBi)
            //.OnDelete(DeleteBehavior.Restrict);
            //=========

            base.OnModelCreating(builder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("server=LEVI\\SQLEXPRESS;database=DV_YTe3;uid=sa;pwd=123;TrustServerCertificate=True");
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
    }
}
