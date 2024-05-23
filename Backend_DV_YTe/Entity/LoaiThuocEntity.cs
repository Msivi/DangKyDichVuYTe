using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("LoaiThuoc")]
    public class LoaiThuocEntity:Entity
    {
        public string tenLoaiThuoc { get; set; }

        public virtual ICollection<ThuocEntity> Thuoc { get; set; }
    }
}
