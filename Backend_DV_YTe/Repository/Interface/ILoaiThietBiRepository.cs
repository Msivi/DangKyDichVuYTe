using Backend_DV_YTe.Entity;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface ILoaiThietBiRepository
    {
        Task<ICollection<LoaiThietBiEntity>> GetAllLoaiThietBi();
        Task<LoaiThietBiEntity> GetLoaiThietBiById(int Id);
        //Task<ICollection<LoaiThietBiEntity>> SearchLoaiThietBi(string searchKey);
        Task<string> CreateLoaiThietBi(LoaiThietBiEntity entity);
        Task UpdateLoaiThietBi(int id, LoaiThietBiEntity entity);
        Task<LoaiThietBiEntity> DeleteLoaiThietBi(int entity, bool isPhysical);
    }
}
