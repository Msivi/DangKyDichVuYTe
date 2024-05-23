using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("LoaiDichVu")]
    public class LoaiDichVuEntity:Entity
    {
        public string tenLoai { get; set; }
 
        public virtual ICollection<DichVuEntity> DichVu { get; set; }
    }
}
