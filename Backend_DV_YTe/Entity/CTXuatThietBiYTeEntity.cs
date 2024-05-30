using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("CTXuatThietBiYTe")]
    public class CTXuatThietBiYTeEntity: CTEntity
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("XuatThietBiYTe")]
        public int MaXuatThietBiYTe { get; set; }
        public virtual XuatThietBiYTeEntity XuatThietBiYTe { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("ThietBiYTe")]
        public int MaThietBiYTe { get; set; }
        public virtual ThietBiYTeEntity ThietBiYTe { get; set; }

        public int soLuong { get; set; }
        //public DateTime ngayTao { get; set; }
    }
}
