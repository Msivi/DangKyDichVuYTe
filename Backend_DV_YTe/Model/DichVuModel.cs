using Backend_DV_YTe.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Model
{
    public class DichVuModel
    {
        public string tenDichVu { get; set; }
        public string moTa { get; set; }
        public double gia { get; set; }
        //public string? hinhAnh { get; set; }
        public int MaLoaiDichVu { get; set; }
        public int? MaChuyenKhoa { get; set; }

    }
}

