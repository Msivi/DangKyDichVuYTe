using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("CTNhapThuoc")]
    public class CTNhapThuocEntity: CTEntity
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("Thuoc")]
        public int MaThuoc { get; set; }
        public virtual ThuocEntity Thuoc { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("NhapThuoc")]
        public int MaNhapThuoc { get; set; }
        public virtual NhapThuocEntity NhapThuoc { get; set; }

        public int soLuong { get; set; }
         
    }
}
