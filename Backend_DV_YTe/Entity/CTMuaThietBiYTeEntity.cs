using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("CTMuaThietBiYTe")]
    public class CTMuaThietBiYTeEntity: CTEntity
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("HoaDon")]
        public int MaHoaDon { get; set; }
        public virtual HoaDonEntity HoaDon { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("ThietBiYTe")]
        public int MaThietBiYTe { get; set; }
        public virtual ThietBiYTeEntity ThietBiYTe { get; set; }

        public int soLuong { get; set; }
        public double donGia { get; set; }
        public double thanhTien { get; set; }

        
    }
}
