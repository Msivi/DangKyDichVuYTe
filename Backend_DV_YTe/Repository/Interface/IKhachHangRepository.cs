using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface IKhachHangRepository
    {
        Task<ICollection<KhachHangEntity>> GetAllKhachHang();
        Task<KhachHangEntity> GetKhachHangById(int Id);
        Task<ICollection<KhachHangEntity>> SearchKhachHang(string searchKey);
        Task<KhachHangEntity> GetTTKhachHang();
        Task UpdateKhachHang(KhachHangModel entity);
        Task UpdateAvatar(string avatarUrl);

        //Task<KhachHangEntity> DeleteKhachHang(string entity, bool isPhysical);
    }
}
