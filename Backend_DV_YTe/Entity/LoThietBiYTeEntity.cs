using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("LoThietBiYTe")]
    public class LoThietBiYTeEntity : Entity
    {

        public DateTime ngaySanXuat { get; set; }
        public DateTime ngayHetHan { get; set; }
        public int soLuong { get; set; } // Số lượng nhập cho lô này
        public string nhaCungCap { get; set; }
        public double donGiaBan { get; set; }

        [ForeignKey("ThietBiYTe")]
        public int? MaThietbiYTe { get; set; }
        public virtual ThietBiYTeEntity ThietBiYTe { get; set; }
        public virtual ICollection<CTNhapThietBiYTeEntity> CTNhapThietBiYTe { get; set; }
    }
}
