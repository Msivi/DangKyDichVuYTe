using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("CTNhapThietBiYTe")]
    public class CTNhapThietBiYTeEntity: CTEntity
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("ThietBiYTe")]
        public int MaThietBiYTe { get; set; }
        public virtual ThietBiYTeEntity ThietBiYTe { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("NhapThietBiYTe")]
        public int MaNhapThietBiYTe { get; set; }
        public virtual NhapThietBiYTeEntity NhapThietBiYTe { get; set; }

        [ForeignKey("LoThietBi")]
        public int? MaLoThietBi { get; set; }
        public virtual LoThietBiYTeEntity LoThietBi { get; set; }

        public int soLuong{ get; set; }
        public double donGiaNhap { get; set; }


    }
}
