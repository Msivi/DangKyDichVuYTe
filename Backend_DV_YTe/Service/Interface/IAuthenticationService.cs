using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Service.Interface
{
    public interface IAuthenticationService
    {
        string GenerateToken (KhachHangEntity user);
        //string EncryptPassword(string password);
        //bool CompareEncryptedPasswords(string password, byte[] storedPassword);
        //Task<dynamic> Login(LoginModel entity);
        //Task<dynamic> ChangePassword(ChangePasswordRequest entity);
        //Task<string> CreateKhachHang(KhachHangModel entity);
        string GenerateToken(NhanVienEntity user);
        string GenerateToken(BacSiEntity bacSi);
        Task<dynamic> ChangePassword(ChangePasswordRequest entity);
        Task<dynamic> Login(LoginModel entity);
        Task<string> CreateKhachHang(KhachHangModel entity);
        Task<string> CreateBacSi(BacSiModel entity);
        Task<string> CreateNhanVien(NhanVienModel entity);
        Task<string> CreateQuanLy(NhanVienModel entity);
    }
}
