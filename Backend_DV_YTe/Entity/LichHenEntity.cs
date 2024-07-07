using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("LichHen")]
    public class LichHenEntity:Entity
    {
        public string? diaDiem { get; set; }
        public string? trangThai { get; set; }
        public string? ghiChu { get; set; }
        public DateTime thoiGianDuKien { get; set; }

        public virtual ICollection<KetQuaDichVuEntity> KetQuaDichVu { get; set; }
        

        [ForeignKey("KhachHang")]
        public int MaKhachHang { get; set; }
        public virtual KhachHangEntity KhachHang { get; set; }

        [ForeignKey("BacSi")]
        public int MaBacSi { get; set; }
        public virtual BacSiEntity BacSi { get; set; }

        [ForeignKey("DichVu")]
        public int MaDichVu { get; set; }
        public virtual DichVuEntity DichVu { get; set; }

        public virtual ICollection<ThanhToanDVEntity> ThanhToanDV { get; set; }
    }
}
