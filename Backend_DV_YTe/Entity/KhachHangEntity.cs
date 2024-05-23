using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("KhachHang")]
    public class KhachHangEntity
    {
        [Key]
         
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maKhachHang { get; set; }
        public string tenKhachHang { get; set; }
        public string? Avatar { get; set; }
        public string email { get; set; }
        public string matKhau { get; set; }
        public string SDT { get; set; }
        public string CMND { get; set; }
        public string Role { get; set; } = "KhachHang";
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public virtual ICollection<LichHenEntity> LichHen { get; set; }

    }
}
