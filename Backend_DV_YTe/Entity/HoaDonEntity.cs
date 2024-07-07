using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("HoaDon")]
    public class HoaDonEntity:Entity
    {
        public DateTime ngayMua { get; set; }
        public double tongTien { get; set; }
        public string trangThai { get; set; }
        public string? ghiChu { get; set; }
        public string? diaChi {get; set; }

        public virtual ICollection<CTMuaThuocEntity> CTMuaThuoc { get; set; }
        public virtual ICollection<CTMuaThietBiYTeEntity> CTMuaThietBiYTe { get; set; }

        [ForeignKey("KhachHang")]
        public int MaKhachHang { get; set; }
        public virtual KhachHangEntity KhachHang { get; set; }
    }
}
