using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface IThuocRepository
    {
        Task<ICollection<ThuocEntity>> GetAllThuoc();
        Task<ThuocEntity> GetThuocById(int Id);
        Task<ICollection<ThuocEntity>> SearchThuoc(string searchKey);
        Task<string> CreateThuoc(ThuocEntity entity);
        Task UpdateThuoc(int id, ThuocEntity entity);
        Task<ThuocEntity> DeleteThuoc(int entity, bool isPhysical);
        Task<ICollection<AllThuocModel>> GetThuocByLoaiThuoc(int entity);

        Task<byte[]> DownloadPdfFile(int entity);
    }
}
