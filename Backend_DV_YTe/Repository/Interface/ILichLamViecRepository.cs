using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface ILichLamViecRepository
    {
         
        Task<ICollection<LichLamViecEntity>> GetAllLichLamViec();
        Task<LichLamViecEntity> GetLichLamViecById(int Id);
        Task<ICollection<LichLamViecEntity>> GetLichLamViecByIdBacSi(int id);
        Task<ICollection<LichLamViecEntity>> GetTTLichLamViecBacSi();
        Task<string> CreateLichLamViec(LichLamViecEntity entity);
        Task UpdateLichLamViec(int id, LichLamViecModel entity);
        Task<LichLamViecEntity> DeleteLichLamViec(int entity, bool isPhysical);

    }
}
