namespace Backend_DV_YTe.Model
{
    public class PhieuHuyLoThuocModel
    {
        public DateTime NgayHuy { get; set; }
      
        public string LyDoHuy { get; set; }
        public List<LoThuocInfo> LoThuocDetails { get; set; }

        public class LoThuocInfo
        {
            public string TenThuoc { get; set; }
            public DateTime NgayHetHan { get; set; }
            public int SoLuong { get; set; }
        }
    }
}
