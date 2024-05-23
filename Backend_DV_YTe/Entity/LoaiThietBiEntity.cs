using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("LoaiThietBi")]
    public class LoaiThietBiEntity:Entity
    {
        public string tenLoaiThietBi { get; set; }

        public virtual ICollection<ThietBiYTeEntity> ThietBiYTe { get; set; }



    }
}
