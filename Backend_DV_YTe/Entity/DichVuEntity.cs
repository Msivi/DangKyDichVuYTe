using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("DichVu")]
    public class DichVuEntity:Entity
    {
        public string tenDichVu { get; set; }
        public string moTa { get; set; }
        public double gia { get; set; }
        public string? hinhAnh { get; set; }
        public virtual ICollection<LichHenEntity> LichHen { get; set; }

        [ForeignKey("LoaiDichVu")]
        public int MaLoaiDichVu { get; set; }
        public virtual LoaiDichVuEntity LoaiDichVu { get; set; }

        [ForeignKey("ChuyenKhoa")]
        public int? MaChuyenKhoa { get; set; }
        public virtual ChuyenKhoaEntity ChuyenKhoa { get; set; }


    }
}
