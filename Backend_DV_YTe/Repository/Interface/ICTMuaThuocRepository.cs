using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface ICTMuaThuocRepository
    {
        Task<ICollection<CTMuaThuocEntity>> GetAllCTMuaThuoc();
        Task<CTMuaThuocEntity> GetCTMuaThuocById(int maHD, int maThuoc);
        Task<string> CreateCTMuaThuoc(CTMuaThuocEntity entity);
        Task UpdateCTMuaThuoc(int maHD, int maThuoc, CTMuaThuocModel entity);
        Task<CTMuaThuocEntity> DeleteCTMuaThuoc(int maHD, int maThuoc, bool isPhysical);
    }
}
