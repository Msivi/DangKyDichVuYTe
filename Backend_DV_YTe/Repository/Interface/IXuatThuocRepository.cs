using Backend_DV_YTe.Entity;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface IXuatThuocRepository
    {
        Task<ICollection<XuatThuocEntity>> GetAllXuatThuoc();
        Task<XuatThuocEntity> GetXuatThuocById(int Id);
        //Task<ICollection<XuatThuocEntity>> SearchXuatThuoc(string searchKey);
        Task<string> CreateXuatThuoc(XuatThuocEntity entity);
        Task UpdateXuatThuoc(int id, XuatThuocEntity entity);
        Task<XuatThuocEntity> DeleteXuatThuoc(int entity, bool isPhysical);
    }
}
