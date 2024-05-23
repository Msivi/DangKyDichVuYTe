using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface IBacSiRepository
    {
        Task<ICollection<BacSiEntity>> GetAllBacSi();
        Task<BacSiEntity> GetBacSiById(int id);
        Task<ICollection<BacSiEntity>> SearchBacSi(string searchKey);
        Task<string> CreateBacSi(BacSiEntity entity, IFormFile imageFile);
        Task UpdateBacSi(int id, BacSiModel entity, IFormFile imageFile);
        Task<BacSiEntity> DeleteBacSi(int id, bool isPhysical);
        Task<ICollection<BacSiInfoModel>> GetAllTTBacSi();
        Task<ICollection<BacSiInfoModel>> GetBacSiByChuyenKhoa(string tenChuyenKhoa);


    }
}
