using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface IThuocRepository
    {
        Task<ICollection<AllThuocModel>> GetAllThuoc();
        Task<AllThuocModel> GetThuocById(int id);
        Task<ICollection<AllThuocModel>> SearchThuoc(string searchKey);
        Task<string> CreateThuoc(ThuocEntity entity, IFormFile imageFile);
        Task UpdateThuoc(int id, ThuocEntity entity, IFormFile imageFile);
        Task<ThuocEntity> DeleteThuoc(int entity, bool isPhysical);
        Task<ICollection<AllThuocModel>> GetThuocByLoaiThuoc(int entity);
        //Task<ICollection<ThuocEntity>> GetAllThuocByMaLoai(int maL);
        Task<byte[]> DownloadPdfFile(int entity);
    }
}
