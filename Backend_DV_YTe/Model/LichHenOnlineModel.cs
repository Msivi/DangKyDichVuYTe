namespace Backend_DV_YTe.Model
{
    public class LichHenOnlineModel
    {
        public int id { get; set; }
        public string diaDiem { get; set; }
        public string tenDichVu { get; set; }
        public string trangThai { get; set; }
        public DateTime thoiGianDuKien { get; set; }
        public int MaBacSi { get; set; }
        public int MaDichVu { get; set; }
        public string? ghiChu { get; set; }
    }
}
