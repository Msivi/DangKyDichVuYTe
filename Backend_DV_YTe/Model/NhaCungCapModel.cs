using System.ComponentModel.DataAnnotations;

namespace Backend_DV_YTe.Model
{
    public class NhaCungCapModel
    {
        public string tenNhaCungCap { get; set; }
        public string diaChi { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression("^(03|05|07|08|09)\\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ!")]
        public string SDT { get; set; }
    }
}
