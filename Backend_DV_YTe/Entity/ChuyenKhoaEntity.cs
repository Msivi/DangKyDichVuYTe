
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("ChuyenKhoa")]
    public class ChuyenKhoaEntity : Entity
    {
        public string tenChuyenKhoa { get; set; }
        public virtual ICollection<DichVuEntity> DichVu { get; set; }
        public virtual ICollection<CTBacSiEntity> CTBacSi { get; set; }
    }
}
