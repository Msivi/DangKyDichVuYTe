using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface IHoaDonRepository
    {
        Task<ICollection<HoaDonEntity>> GetAllHoaDon();
        //Task<HoaDonEntity> GetHoaDonById(int id);
        //Task<ICollection<HoaDonEntity>> SearchHoaDon(string searchKey);
        Task<string> CreateHoaDon();
        //Task UpdateHoaDon(int id, HoaDonModel entity);
        Task<HoaDonEntity> DeleteHoaDon(int id, bool isPhysical);
    }
}
