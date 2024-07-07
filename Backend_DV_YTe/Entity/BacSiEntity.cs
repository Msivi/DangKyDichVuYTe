using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("BacSi")]
    public class BacSiEntity:Entity
    {
        public string tenBacSi { get; set; }
        public string bangCap { get;set; }
        public string? hinhAnh { get; set; }
        public string email { get; set; }
        public string matKhau { get; set; }
        public string Role { get; set; } = "BacSi";
        public virtual ICollection<LichHenEntity> LichHen { get; set; }
        public virtual ICollection<CTBacSiEntity> CTBacSi { get; set; }
        public virtual ICollection<LichLamViecEntity> LichLamViec { get; set; }
    }
}
