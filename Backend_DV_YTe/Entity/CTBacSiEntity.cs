using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_DV_YTe.Entity
{
    [Table("CTBacSi")]
    public class CTBacSiEntity:CTEntity
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("BacSi")]
        public int MaBacSi { get; set; }
        public virtual BacSiEntity BacSi { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("ChuyenKhoa")]
        public int MaChuyenKhoa { get; set; }
        public virtual ChuyenKhoaEntity ChuyenKhoa { get; set; }

        

    }
}
