using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("DanhGia")]
    public class DanhGiaEntity:Entity
    {
        public string noiDungDanhGia { get; set; }
        public double soSaoDanhGia { get; set; }
        public string trangThai { get; set; } = "0";
        public string? hinhAnh { get; set; }

        [ForeignKey("KetQuaDichVu")]
        public int MaKetQuaDichVu { get; set; }
        public virtual KetQuaDichVuEntity KetQuaDichVu { get; set; }

        //[ForeignKey("NhanVien")]
        //public int? MaNhanVien { get; set; }
        //public virtual NhanVienEntity NhanVien { get; set; }

    }
}
