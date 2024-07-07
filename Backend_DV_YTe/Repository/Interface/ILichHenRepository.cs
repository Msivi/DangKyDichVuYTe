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
        Task UpdateHuyLichHen(int id);
        Task<LichHenEntity> DeleteLichHen(int entity, bool isPhysical);
        Task<ICollection<LichHenOnlineModel>> GetAllLichHenOnline();
        Task UpdateLichHenNhanVien(int id, string diaDiem);
        Task<ICollection<LichHenEntity>> GetAllLichHenKhachHang();
        Task<ICollection<LichHenEntity>> GetAllLichHenBacSi();
        Task<ICollection<LichHenEntity>> GetAllLichHenBacSiDaKham();
        Task<LichHenEntity> GetLichHenByMaBacSi(int id, DateTime ngayDK, string trangThai);
    }
}
