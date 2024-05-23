using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("CTMuaThuoc")]
    public class CTMuaThuocEntity: CTEntity
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("HoaDon")]
        public int MaHoaDon { get; set; }
        public virtual HoaDonEntity HoaDon { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("Thuoc")]
        public int MaThuoc { get; set; }
        public virtual ThuocEntity Thuoc { get; set; }

        public int soLuong { get; set; }
        public double donGia { get; set; }
        public double thanhTien { get; set;}

        
    }
}
