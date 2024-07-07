using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("LoThuoc")]
    public class LoThuocEntity:Entity
    {
       
        public DateTime ngaySanXuat { get; set; }
        public DateTime ngayHetHan { get; set; }
        public int soLuong { get; set; } // Số lượng nhập cho lô này
        public string nhaCungCap { get; set; }
        public double donGiaBan { get; set; }

        [ForeignKey("Thuoc")]
        public int MaThuoc { get; set; }
        public virtual ThuocEntity Thuoc { get; set; }
        public virtual ICollection<CTNhapThuocEntity> CTNhapThuoc { get; set; }
        public virtual ICollection<HuyLoThuocEntity> HuyLoThuoc { get; set; }
    }
}
