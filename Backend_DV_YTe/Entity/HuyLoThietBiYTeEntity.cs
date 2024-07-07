using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("HuyLoThietBiYTe")]
    public class HuyLoThietBiYTeEntity:Entity
    {
        [ForeignKey("LoThietBiYTe")]
        public int LoThietBiId { get; set; }
        public virtual LoThietBiYTeEntity LoThietBiYTe { get; set; }

        public DateTime NgayHuy { get; set; }
        public string LyDoHuy { get; set; }
    }
}
