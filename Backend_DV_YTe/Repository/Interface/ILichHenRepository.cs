using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface ILichHenRepository
    {
        Task<ICollection<LichHenEntity>> GetAllLichHen();
        Task<LichHenEntity> GetLichHenById(int Id);
        //Task<ICollection<LichHenEntity>> SearchLichHen(string searchKey);
        Task<string> CreateLichHen(LichHenEntity entity);
        Task UpdateLichHen(int id, LichHenModel entity);
        Task<LichHenEntity> DeleteLichHen(int entity, bool isPhysical);
        Task<ICollection<LichHenEntity>> GetAllLichHenOnline();
        Task UpdateLichHenNhanVien(int id, string diaDiem, DateTime thoiGianDuKien);
        Task<ICollection<LichHenEntity>> GetAllLichHenKhachHang();
    }
}
