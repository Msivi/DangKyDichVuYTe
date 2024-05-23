using Backend_DV_YTe.Entity;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface INhaCungCapRepository
    {
        Task<ICollection<NhaCungCapEntity>> GetAllNhaCungCap();
        Task<NhaCungCapEntity> GetNhaCungCapById(int Id);
        Task<ICollection<NhaCungCapEntity>> SearchNhaCungCap(string searchKey);
        Task<string> CreateNhaCungCap(NhaCungCapEntity entity);
        Task UpdateNhaCungCap(int id, NhaCungCapEntity entity);
        Task<NhaCungCapEntity> DeleteNhaCungCap(int entity, bool isPhysical);
    }
}
