namespace Backend_DV_YTe.Model
{
    public class BacSiInfoModel
    {
        public int Id { get; set; }
        public string TenBacSi { get; set; }
        public string BangCap { get; set; }
        public string? HinhAnh { get; set; }
        public List<string> ChuyenKhoa { get; set; }
    }
}
