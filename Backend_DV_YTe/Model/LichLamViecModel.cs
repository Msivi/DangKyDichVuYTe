using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Model
{
    public class LichLamViecModel
    {
        public DateTime Ngay { get; set; }
        public DateTime GioBatDau { get; set; }
        public DateTime GioKetThuc { get; set; }
        public int MaBacSi { get; set; }
    }
}
