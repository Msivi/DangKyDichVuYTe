using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("HuyLoThuoc")]
    public class HuyLoThuocEntity:Entity
    {
        [ForeignKey("LoThuoc")]
        public int LoThuocId { get; set; }
        public virtual LoThuocEntity LoThuoc { get; set; }

        public DateTime NgayHuy { get; set; }
        public string LyDoHuy { get; set; }
    }
}
