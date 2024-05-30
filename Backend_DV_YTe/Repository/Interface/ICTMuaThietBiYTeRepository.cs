using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface ICTMuaThietBiYTeRepository
    {
        Task<ICollection<CTMuaThietBiYTeEntity>> GetAllCTMuaThietBiYTe();
        Task<CTMuaThietBiYTeEntity> GetCTMuaThietBiYTeById(int maHD, int maThuoc);
        Task<string> CreateCTMuaThietBiYTe(CTMuaThietBiYTeEntity entity);
        Task UpdateCTMuaThietBiYTe(int maHD, int maThuoc, CTMuaThietBiYTeModel entity);
        Task<CTMuaThietBiYTeEntity> DeleteCTMuaThietBiYTe(int maHD, int maThuoc, bool isPhysical);
    }
}
