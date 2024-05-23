using Backend_DV_YTe.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Model
{
    public class DanhGiaModel
    {
        public string noiDungDanhGia { get; set; }
        public double soSaoDanhGia { get; set; }
        //public string trangThai { get; set; } = "0";
        //public string? hinhAnh { get; set; }

        public int MaKetQuaDichVu { get; set; }
        //public int? MaNhanVien { get; set; }

    }
}
