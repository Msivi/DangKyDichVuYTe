using Backend_DV_YTe.Entity;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface IXuatThietBiYTeRepository
    {
        Task<ICollection<XuatThietBiYTeEntity>> GetAllXuatThietBiYTe();
        Task<XuatThietBiYTeEntity> GetXuatThietBiYTeById(int Id);
        //Task<ICollection<XuatThietBiYTeEntity>> SearchXuatThietBiYTe(string searchKey);
        Task<string> CreateXuatThietBiYTe(XuatThietBiYTeEntity entity);
        Task UpdateXuatThietBiYTe(int id, XuatThietBiYTeEntity entity);
        Task<XuatThietBiYTeEntity> DeleteXuatThietBiYTe(int entity, bool isPhysical);
    }
}
