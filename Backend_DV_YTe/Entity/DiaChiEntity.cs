using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("DiaChi")]
    public class DiaChiEntity:Entity
    {
        public string tenDiaChi { get; set; }
        public bool IsDefault { get; set; }

        [ForeignKey("KhachHang")]
        public int KhachHangId { get; set; }
        public virtual KhachHangEntity KhachHang { get; set; }

    }
}
