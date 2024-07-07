namespace Backend_DV_YTe.Mapper
{
    public static class AutoMapper
    {
        public static IServiceCollection AddAutoMapperServices(this IServiceCollection services)
        {
            services.AddAutoMapper(c =>
            {
                c.AddProfile(new KhachHangProfile());
                c.AddProfile(new NhaCungCapProfile());
                c.AddProfile(new NhanVienProfile());
                c.AddProfile(new NhapThuocProfile());
                c.AddProfile(new LoaiThuocProfile());
                c.AddProfile(new ThuocProfile());
                c.AddProfile(new CTNhapThuocProfile());
                c.AddProfile(new XuatThuocProfile());
                c.AddProfile(new CTXuatThuocProfile());
                c.AddProfile(new NhapThietBiYTeProfile());
                c.AddProfile(new ThietBiYTeProfile());
                c.AddProfile(new CTNhapThietBiYTeProfile());
                c.AddProfile(new XuatThietBiYTeProfile());
                c.AddProfile(new CTXuatThietBiYTeProfile());
                c.AddProfile(new LoaiDichVuProfile());
                c.AddProfile(new LoaiThietBiProfile());
                c.AddProfile(new DichVuProfile());
                c.AddProfile(new BacSiProfile());
                c.AddProfile(new KetQuaDichVuProfile());
                c.AddProfile(new LichHenProfile());
                c.AddProfile(new DanhGiaProfile());
                c.AddProfile(new ChuyenKhoaProfile());
                c.AddProfile(new CTBacSiProfile());
                c.AddProfile(new CTMuaThuocProfile());
                c.AddProfile(new CTMuaThietBiYTeProfile());
                c.AddProfile(new DiaChiProfile());
                c.AddProfile(new LichLamViecProfile());
            });
            return services;
        }
    }
}
