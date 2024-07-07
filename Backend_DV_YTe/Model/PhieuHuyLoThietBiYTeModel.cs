namespace Backend_DV_YTe.Model
{
    public class PhieuHuyLoThietBiYTeModel
    {
        public DateTime NgayHuy { get; set; }

        public string LyDoHuy { get; set; }
        public List<LoThietBiInfo> LoThietBiDetails { get; set; }

        public class LoThietBiInfo
        {
            public string TenThietBi { get; set; }
            public DateTime NgayHetHan { get; set; }
            public int SoLuong { get; set; }
        }
    }
}
