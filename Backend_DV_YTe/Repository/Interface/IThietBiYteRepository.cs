using Backend_DV_YTe.Entity;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface IThietBiYteRepository
    {
        Task<ICollection<ThietBiYTeEntity>> GetAllThietBiYTe();
        Task<ThietBiYTeEntity> GetThietBiYTeById(int Id);
        Task<ICollection<ThietBiYTeEntity>> SearchThietBiYTe(string searchKey);
        Task<string> CreateThietBiYTe(ThietBiYTeEntity entity);
        Task UpdateThietBiYTe(int id, ThietBiYTeEntity entity);
        Task<ThietBiYTeEntity> DeleteThietBiYTe(int entity, bool isPhysical);
    }
}
