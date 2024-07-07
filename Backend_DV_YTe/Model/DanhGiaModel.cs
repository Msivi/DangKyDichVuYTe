using Backend_DV_YTe.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Model
{
    public class DanhGiaModel
    {
        [Required(ErrorMessage = "Nội dung đánh giá là bắt buộc")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nội dung đánh giá phải có ít nhất 3 ký tự và tối đa 100 ký tự")]
        public string noiDungDanhGia { get; set; }
        public double soSaoDanhGia { get; set; }
        //public string trangThai { get; set; } = "0";
        //public string? hinhAnh { get; set; }

        public int MaKetQuaDichVu { get; set; }
        //public int? MaNhanVien { get; set; }

    }
}
