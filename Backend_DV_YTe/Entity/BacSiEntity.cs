using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("BacSi")]
    public class BacSiEntity:Entity
    {
        public string tenBacSi { get; set; }
        public string bangCap { get;set; }
        public string? hinhAnh { get; set; }

        public virtual ICollection<LichHenEntity> LichHen { get; set; }
        public virtual ICollection<CTBacSiEntity> CTBacSi { get; set; }
    }
}
