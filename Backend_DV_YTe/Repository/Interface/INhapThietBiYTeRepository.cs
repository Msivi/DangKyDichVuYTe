using Backend_DV_YTe.Entity;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface INhapThietBiYTeRepository
    {
        Task<ICollection<NhapThietBiYTeEntity>> GetAllNhapThietBiYTe();
        Task<NhapThietBiYTeEntity> GetNhapThietBiYTeById(int Id);
        //Task<ICollection<NhapThietBiYTeEntity>> SearchNhapThietBiYTe(string searchKey);
        Task<string> CreateNhapThietBiYTe(NhapThietBiYTeEntity entity);
        Task UpdateNhapThietBiYTe(int id, NhapThietBiYTeEntity entity);
        Task<NhapThietBiYTeEntity> DeleteNhapThietBiYTe(int entity, bool isPhysical);
    }
}
