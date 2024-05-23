using Backend_DV_YTe.Entity;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface ILoaiDichVuRepository
    {
        Task<ICollection<LoaiDichVuEntity>> GetAllLoaiDichVu();
        Task<LoaiDichVuEntity> GetLoaiDichVuById(int Id);
        //Task<ICollection<LoaiDichVuEntity>> SearchLoaiDichVu(string searchKey);
        Task<string> CreateLoaiDichVu(LoaiDichVuEntity entity);
        Task UpdateLoaiDichVu(int id, LoaiDichVuEntity entity);
        Task<LoaiDichVuEntity> DeleteLoaiDichVu(int entity, bool isPhysical);
    }
}
