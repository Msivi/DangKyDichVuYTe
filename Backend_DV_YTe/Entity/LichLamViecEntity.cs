using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("LichLamViec")]
    public class LichLamViecEntity:Entity
    {
        
        public DateTime Ngay { get; set; }
        public DateTime GioBatDau { get; set; }
        public DateTime GioKetThuc { get; set; }

        [ForeignKey("BacSi")]
        public int MaBacSi { get; set; }
        public virtual BacSiEntity BacSi { get; set; }
    }
}
