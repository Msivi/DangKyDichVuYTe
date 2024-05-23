using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface IDanhGiaRepository
    {
        Task<ICollection<DanhGiaEntity>> GetAllDanhGiaChuaDuyet();
        Task<ICollection<DanhGiaEntity>> GetAllDanhGiaDaDuyet();
        Task<DanhGiaEntity> GetDanhGiaById(int id);
        //Task<ICollection<DanhGiaEntity>> SearchDanhGia(string searchKey);
        //Task<string> CreateDanhGia(DanhGiaEntity entity);
        Task<string> CreateDanhGia(DanhGiaEntity entity, IFormFile imageFile);
        Task UpdateDanhGia(int id, DanhGiaModel entity, IFormFile imageFile);
        Task DuyetDanhGia(List<int> ids);
        Task<DanhGiaEntity> DeleteDanhGia(int id, bool isPhysical);
        Task<List<DichVuDanhGia>> GetDichVuDanhGia(int maDichVu);
        Task<List<DanhGiaEntity>> GetAllDanhGiaByMaDichVu(int maDichVu);
    }
}
