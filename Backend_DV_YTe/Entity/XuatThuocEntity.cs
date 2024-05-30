using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("XuatThuoc")]
    public class XuatThuocEntity:Entity
    {
        public DateTime ngayTao { get; set; }

        //[ForeignKey("NhanVien")]
        //public int MaNhanVien { get; set; }
        //public virtual NhanVienEntity NhanVien { get; set; }

        public virtual ICollection<CTXuatThuocEntity> CTXuatThuoc { get; set; }

    }
}
