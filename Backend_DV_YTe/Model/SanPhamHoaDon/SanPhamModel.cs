namespace Backend_DV_YTe.Model.SanPhamHoaDon
{
    public class SanPhamModel
    {
        public int maSanPham { get; set; }
        public string tenSanPham { get; set; }
        public int soLuong { get; set; }
        public double donGia { get; set; }
        public double thanhTien { get; set; }
        public string? hinhAnh { get; set; }
    }
}
