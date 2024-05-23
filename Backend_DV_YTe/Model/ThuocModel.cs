using Backend_DV_YTe.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Model
{
    public class ThuocModel
    {
        public string tenThuoc { get; set; }
        public string donViTinh { get; set; }
        public double donGia { get; set; }
       // public int? soLuong { get; set; }
        public DateTime ngaySanXuat { get; set; }
        public DateTime ngayHetHan { get; set; }
        public string nhaSanXuat { get; set; }
        public int MaLoaiThuoc { get; set; }
        public string? thanhPhan { get; set; }
        public string? moTa { get; set; }

    }
}
