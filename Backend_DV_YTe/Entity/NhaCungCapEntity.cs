using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("NhaCungCap")]
    public class NhaCungCapEntity:Entity
    {
        public string tenNhaCungCap { get; set; }
        public string diaChi { get; set; }
        public string SDT { get; set; }

        public virtual ICollection<NhapThuocEntity> NhapThuoc { get; set; }
        public virtual ICollection<NhapThietBiYTeEntity> NhapThietBiYTe { get; set; }
    }
}
