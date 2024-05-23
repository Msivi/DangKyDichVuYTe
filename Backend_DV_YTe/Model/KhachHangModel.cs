using System.ComponentModel.DataAnnotations;

namespace Backend_DV_YTe.Model
{
    public class KhachHangModel
    {
        //public int maKhachHang { get; set; }
        public string tenKhachHang { get; set; }
        //public string? Avatar { get; set; }
        public string email { get; set; }
        public string matKhau { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression("^(03|05|07|08|09)\\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ!")]
        public string SDT { get; set; }

        [RegularExpression("^[0-9]{9}$|^[0-9]{12}$", ErrorMessage = "Invalid ID")]
        [StringLength(12, MinimumLength = 9, ErrorMessage = "Số chứng minh phải là 9 hoặc 12 số!")]
        public string CMND { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }

    }
}
