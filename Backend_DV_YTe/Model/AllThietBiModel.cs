namespace Backend_DV_YTe.Model
{
    public class AllThietBiModel
    {
        public int id { get; set; }
        public string tenThietBiYTe { get; set; }
        public double donGia { get; set; }
        public int soLuong { get; set; }
        public DateTime ngaySanXuat { get; set; }
        public DateTime ngayHetHan { get; set; }
        public string nhaSanXuat { get; set; }
        public string tenDanhMuc { get; set; }
        public string? moTa { get; set; }
        public string? hinhAnh { get; set; }
    }
}
