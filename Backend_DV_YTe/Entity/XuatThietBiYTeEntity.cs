using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("XuatThietBiYTe")]
    public class XuatThietBiYTeEntity:Entity
    {
        public DateTime ngayTao { get; set; }

        //[ForeignKey("NhanVien")]
        //public int MaNhanVien { get; set; }
        //public virtual NhanVienEntity NhanVien { get; set; }

        public virtual ICollection<CTXuatThietBiYTeEntity> CTXuatThietBiYTe { get; set; }
    }
}
