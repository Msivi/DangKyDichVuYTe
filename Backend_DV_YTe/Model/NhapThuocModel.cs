using Backend_DV_YTe.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Model
{
    public class NhapThuocModel
    {
        public DateTime ngayTao { get; set; }
        //public int MaNhanVien { get; set; }
        public int MaNhaCungCap { get; set; }
        
    }
}
