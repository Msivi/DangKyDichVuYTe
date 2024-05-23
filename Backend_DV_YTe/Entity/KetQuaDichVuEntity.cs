using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("KetQuaDichVu")]
    public class KetQuaDichVuEntity:Entity
    {
        public string moTa { get; set; }
        public virtual ICollection<DanhGiaEntity> DanhGia { get; set; }

        [ForeignKey("LichHen")]
        public int MaLichHen { get; set; }
        public virtual LichHenEntity LichHen { get; set; }

        [ForeignKey("NhanVien")]
        public int MaNhanVien { get; set; }
        public virtual NhanVienEntity NhanVien { get; set; }

       
    }
}
