using System.ComponentModel.DataAnnotations;

namespace Backend_DV_YTe.Model
{
    public class BacSiModel
    {
        public string tenBacSi { get; set; }
        public string bangCap { get; set; }
        public string email { get; set; }
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string matKhau { get; set; }
        //public string chuyenKhoa { get; set; }
    }
}
