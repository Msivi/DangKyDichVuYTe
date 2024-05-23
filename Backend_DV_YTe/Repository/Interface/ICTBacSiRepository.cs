using Backend_DV_YTe.Entity;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface ICTBacSiRepository
    {
        Task<ICollection<CTBacSiEntity>> GetAllCTBacSi();
        Task<CTBacSiEntity> GetCTBacSiById(int maBS, int maCK);
        //Task<ICollection<CTBacSiEntity>> SearchCTBacSi(string searchKey);
        Task<string> CreateCTBacSi(CTBacSiEntity entity);
        Task UpdateCTBacSi(int maBS, int maCK, CTBacSiEntity entity);
        Task<CTBacSiEntity> DeleteCTBacSi(int maBS, int maCK, bool isPhysical);
       
    }
}
