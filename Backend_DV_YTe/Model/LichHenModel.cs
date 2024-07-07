using Backend_DV_YTe.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Model
{
    public class LichHenModel
    {
        public string diaDiem { get; set; }
        //public string trangThai { get; set; }
        public DateTime thoiGianDuKien { get; set; }
        public int MaBacSi { get; set; }
        public int MaDichVu { get; set; }
        public string? ghiChu { get; set; }

    }
}
