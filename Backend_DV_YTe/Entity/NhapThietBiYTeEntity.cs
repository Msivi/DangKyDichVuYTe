using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("NhapThietBiYTe")]
    public class NhapThietBiYTeEntity:Entity
    {
        public DateTime ngayTao { get; set; }

        [ForeignKey("NhanVien")]
        public int MaNhanVien { get; set; }
        public virtual NhanVienEntity NhanVien { get; set; }

        [ForeignKey("NhaCungCap")]
        public int MaNhaCungCap { get; set; }
        public virtual NhaCungCapEntity NhaCungCap { get; set; }

        public virtual ICollection<CTNhapThietBiYTeEntity> CTNhapThietBiYTe { get; set; }
    }
}
