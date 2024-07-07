using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface IDiaChiRepository
    {
        Task<ICollection<DiaChiEntity>> GetAllDiaChi();
        Task<DiaChiEntity> GetDiaChiById(int Id);
        //Task<ICollection<DiaChiEntity>> SearchDiaChi(string searchKey);
        Task<string> CreateDiaChi(DiaChiEntity entity);
        Task UpdateDiaChi(int id, DiaChiModel entity);
        Task<DiaChiEntity> DeleteDiaChi(int entity, bool isPhysical);
        Task<string> SetDefaultAddressAsync(int addressId);
        Task<ICollection<DiaChiEntity>> GetDefaultAddressAsync();
    }
}
