using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface IChuyenKhoaRepository
    {
        Task<ICollection<ChuyenKhoaEntity>> GetAllChuyenKhoa();
        Task<ChuyenKhoaEntity> GetChuyenKhoaById(int id);
        Task<ICollection<ChuyenKhoaEntity>> SearchChuyenKhoa(string searchKey);
        Task<string> CreateChuyenKhoa(ChuyenKhoaEntity entity);
        Task UpdateChuyenKhoa(int id, ChuyenKhoaEntity entity);
        Task<ChuyenKhoaEntity> DeleteChuyenKhoa(int id, bool isPhysical);
    }
}
