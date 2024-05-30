using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("CTXuatThuoc")]
    public class CTXuatThuocEntity: CTEntity
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("Thuoc")]
        public int MaThuoc { get; set; }
        public virtual ThuocEntity Thuoc { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("XuatThuoc")]
        public int MaXuatThuoc { get; set; }
        public virtual XuatThuocEntity XuatThuoc { get; set; }

        public int soLuong { get; set; }
        //public DateTime ngayTao { get; set; }
    }
}
