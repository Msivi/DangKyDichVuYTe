using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("ThanhToanDV")]
    public class ThanhToanDVEntity:Entity
    {
        public double tongTien { get; set; }
        public DateTime ngayThanhToan { get; set; }
        public string phuongThucThanhToan { get; set; }
        public string trangThai { get; set; }
        //public string? ghiChu { get; set; }


        [ForeignKey("LichHen")]
        public int MaLichHen { get; set; }
        public virtual LichHenEntity LichHen { get; set; }
    }
}
